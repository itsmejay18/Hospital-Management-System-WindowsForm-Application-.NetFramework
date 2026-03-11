using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using HospitalManagementSystem.DAL;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace HospitalManagementSystem.Helpers
{
    public sealed class DatabaseBackupService
    {
        private const string ManifestFileName = "manifest.json";
        private const string SnapshotFileName = "snapshot-index.json";
        private const string SqlFileName = "backup.sql";

        private readonly string _connectionString;
        private readonly MySqlConnectionStringBuilder _builder;

        public DatabaseBackupService(string connectionString = null)
        {
            _connectionString = string.IsNullOrWhiteSpace(connectionString)
                ? DatabaseConnection.GetActiveConnectionString()
                : connectionString.Trim();

            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new InvalidOperationException("An active database connection is required for backup.");
            }

            _builder = new MySqlConnectionStringBuilder(_connectionString);
            if (string.IsNullOrWhiteSpace(_builder.Database))
            {
                throw new InvalidOperationException("The active connection does not specify a database name.");
            }
        }

        public string SourceServer => _builder.Server;

        public string SourceDatabase => _builder.Database;

        public string GetDefaultBackupRoot()
        {
            try
            {
                var settings = AppSettingsStore.Load();
                if (!string.IsNullOrWhiteSpace(settings?.BackupPath))
                {
                    return NormalizeBackupRoot(settings.BackupPath);
                }
            }
            catch
            {
                // Fall back to a safe local path.
            }

            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return NormalizeBackupRoot(Path.Combine(localAppData, "HospitalManagementSystem", "Backups"));
        }

        public IList<DatabaseBackupSetSummary> ListBackups(string backupRoot)
        {
            var root = NormalizeBackupRoot(backupRoot);
            if (!Directory.Exists(root))
            {
                return new List<DatabaseBackupSetSummary>();
            }

            var backups = new List<DatabaseBackupSetSummary>();
            foreach (var directory in Directory.GetDirectories(root))
            {
                var manifestPath = Path.Combine(directory, ManifestFileName);
                if (!File.Exists(manifestPath))
                {
                    continue;
                }

                try
                {
                    var manifest = JsonConvert.DeserializeObject<DatabaseBackupSetSummary>(File.ReadAllText(manifestPath));
                    if (manifest == null)
                    {
                        continue;
                    }

                    AttachBackupPaths(manifest, directory);
                    backups.Add(manifest);
                }
                catch
                {
                    // Ignore malformed backup folders.
                }
            }

            return backups
                .OrderByDescending(item => item.CreatedAtUtc)
                .ToList();
        }

        public DatabaseBackupSetSummary CreateBackup(
            DatabaseBackupKind requestedKind,
            string backupRoot,
            IProgress<string> progress = null)
        {
            var root = NormalizeBackupRoot(backupRoot);
            Directory.CreateDirectory(root);

            Report(progress, "Loading previous backup metadata...");
            var backups = ListBackups(root)
                .Where(item => string.Equals(item.SourceSignature, BuildSourceSignature(), StringComparison.OrdinalIgnoreCase))
                .OrderBy(item => item.CreatedAtUtc)
                .ToList();

            var lastBackup = backups.LastOrDefault();
            var lastFullBackup = backups.LastOrDefault(item => item.BackupKind == DatabaseBackupKind.Full);

            var actualKind = requestedKind;
            DatabaseBackupSetSummary baseline = null;

            if (requestedKind == DatabaseBackupKind.Incremental)
            {
                baseline = lastBackup;
                if (baseline == null)
                {
                    actualKind = DatabaseBackupKind.Full;
                }
            }
            else if (requestedKind == DatabaseBackupKind.Differential)
            {
                baseline = lastFullBackup;
                if (baseline == null)
                {
                    actualKind = DatabaseBackupKind.Full;
                }
            }

            Report(progress, "Reading database schema and rows...");
            DatabaseSnapshot snapshot;
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                snapshot = CaptureSnapshot(connection, progress);
            }

            var currentIndex = BuildSnapshotIndex(snapshot);
            if (actualKind != DatabaseBackupKind.Full && baseline != null)
            {
                var baselineIndex = LoadSnapshotIndex(baseline.SnapshotPath);
                if (!CanUseDeltaBackup(currentIndex, baselineIndex))
                {
                    actualKind = DatabaseBackupKind.Full;
                    baseline = null;
                    Report(progress, "Schema changed since the baseline backup. Creating a full backup instead.");
                }
            }

            string sqlScript;
            var changedTables = 0;
            var changedRows = 0;
            var totalRows = snapshot.Tables.Sum(table => table.Rows.Count);

            if (actualKind == DatabaseBackupKind.Full)
            {
                Report(progress, "Building full backup script...");
                sqlScript = BuildFullBackupSql(snapshot);
                changedTables = snapshot.Tables.Count;
                changedRows = totalRows;
            }
            else
            {
                Report(progress, "Calculating row changes from baseline...");
                var baselineIndex = LoadSnapshotIndex(baseline.SnapshotPath);
                var deltaPlan = BuildDeltaPlan(snapshot, baselineIndex);
                sqlScript = BuildDeltaBackupSql(deltaPlan, actualKind);
                changedTables = deltaPlan.Tables.Count;
                changedRows = deltaPlan.Tables.Sum(item => item.Rows.Count + item.DeletedKeys.Count);
            }

            var backupId = string.Format(
                CultureInfo.InvariantCulture,
                "{0:yyyyMMddHHmmss}-{1}",
                DateTime.UtcNow,
                Guid.NewGuid().ToString("N").Substring(0, 8));

            var directoryName = string.Format(
                CultureInfo.InvariantCulture,
                "{0:yyyyMMdd_HHmmss}_{1}_{2}",
                DateTime.Now,
                actualKind.ToString().ToLowerInvariant(),
                SanitizePathSegment(SourceDatabase));
            var backupDirectory = Path.Combine(root, directoryName);
            Directory.CreateDirectory(backupDirectory);

            Report(progress, "Writing backup files to disk...");
            var manifest = new DatabaseBackupSetSummary
            {
                BackupId = backupId,
                BackupKind = actualKind,
                SourceServer = SourceServer,
                SourceDatabase = SourceDatabase,
                CreatedAtUtc = DateTime.UtcNow,
                BaseFullBackupId = actualKind == DatabaseBackupKind.Full
                    ? backupId
                    : (actualKind == DatabaseBackupKind.Differential
                        ? baseline?.BackupId
                        : lastFullBackup?.BackupId),
                PreviousBackupId = actualKind == DatabaseBackupKind.Incremental ? baseline?.BackupId : null,
                TotalTables = snapshot.Tables.Count,
                TotalRows = totalRows,
                ChangedTables = changedTables,
                ChangedRows = changedRows,
                IsEmptyDelta = actualKind != DatabaseBackupKind.Full && changedTables == 0
            };

            AttachBackupPaths(manifest, backupDirectory);
            WriteJson(manifest.ManifestPath, manifest);
            WriteJson(manifest.SnapshotPath, currentIndex);
            File.WriteAllText(manifest.SqlScriptPath, sqlScript, Encoding.UTF8);

            Report(progress, "Backup completed.");
            return manifest;
        }

        public void RestoreBackup(string backupRoot, string backupId, IProgress<string> progress = null)
        {
            var root = NormalizeBackupRoot(backupRoot);
            var backups = ListBackups(root)
                .Where(item => string.Equals(item.SourceSignature, BuildSourceSignature(), StringComparison.OrdinalIgnoreCase))
                .ToList();

            var target = backups.FirstOrDefault(item => string.Equals(item.BackupId, backupId, StringComparison.OrdinalIgnoreCase));
            if (target == null)
            {
                throw new InvalidOperationException("The selected backup set could not be found.");
            }

            var restoreChain = ResolveRestoreChain(target, backups);
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                foreach (var backup in restoreChain)
                {
                    if (!File.Exists(backup.SqlScriptPath))
                    {
                        throw new FileNotFoundException("Backup SQL script was not found.", backup.SqlScriptPath);
                    }

                    Report(
                        progress,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Applying {0} backup from {1:yyyy-MM-dd HH:mm:ss}...",
                            backup.BackupKind,
                            backup.CreatedAtUtc.ToLocalTime()));

                    var sql = File.ReadAllText(backup.SqlScriptPath, Encoding.UTF8);
                    if (string.IsNullOrWhiteSpace(sql))
                    {
                        continue;
                    }

                    var script = new MySqlScript(connection, sql);
                    script.Execute();
                }
            }

            Report(progress, "Restore completed.");
        }

        private DatabaseSnapshot CaptureSnapshot(MySqlConnection connection, IProgress<string> progress)
        {
            var snapshot = new DatabaseSnapshot
            {
                DatabaseName = SourceDatabase
            };

            foreach (var tableName in GetBaseTables(connection))
            {
                Report(progress, "Reading table " + tableName + "...");
                var table = new DatabaseTableSnapshot
                {
                    TableName = tableName,
                    CreateTableSql = GetCreateTableSql(connection, tableName)
                };
                table.PrimaryKeyColumns.AddRange(GetPrimaryKeyColumns(connection, tableName));
                table.Columns.AddRange(GetTableColumns(connection, tableName));
                LoadRows(connection, table);
                snapshot.Tables.Add(table);
            }

            foreach (var viewName in GetViews(connection))
            {
                snapshot.Views.Add(new DatabaseViewSnapshot
                {
                    ViewName = viewName,
                    CreateViewSql = SanitizeCreateViewSql(GetCreateViewSql(connection, viewName))
                });
            }

            return snapshot;
        }

        private static void LoadRows(MySqlConnection connection, DatabaseTableSnapshot table)
        {
            var sql = new StringBuilder();
            sql.Append("SELECT * FROM ");
            sql.Append(QuoteIdentifier(table.TableName));
            if (table.PrimaryKeyColumns.Count > 0)
            {
                sql.Append(" ORDER BY ");
                sql.Append(string.Join(", ", table.PrimaryKeyColumns.Select(QuoteIdentifier)));
            }

            using (var command = new MySqlCommand(sql.ToString(), connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var row = new DatabaseTableRow();
                    foreach (var column in table.Columns)
                    {
                        var rawValue = reader[column];
                        row.Values[column] = rawValue == DBNull.Value ? null : CloneValue(rawValue);
                    }

                    row.Hash = ComputeRowHash(row.Values, table.Columns);
                    if (table.PrimaryKeyColumns.Count > 0)
                    {
                        var keyData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        foreach (var keyColumn in table.PrimaryKeyColumns)
                        {
                            keyData[keyColumn] = NormalizeKeyComponent(row.Values.ContainsKey(keyColumn) ? row.Values[keyColumn] : null);
                        }

                        row.PrimaryKeySignature = JsonConvert.SerializeObject(keyData, Formatting.None);
                        table.RowHashes[row.PrimaryKeySignature] = row.Hash;
                    }

                    table.Rows.Add(row);
                }
            }

            var hashSource = table.PrimaryKeyColumns.Count > 0
                ? table.RowHashes.Values
                : table.Rows.Select(item => item.Hash);
            table.TableHash = ComputeTableHash(hashSource);
        }

        private static BackupSnapshotIndex BuildSnapshotIndex(DatabaseSnapshot snapshot)
        {
            var index = new BackupSnapshotIndex
            {
                DatabaseName = snapshot.DatabaseName
            };

            foreach (var table in snapshot.Tables)
            {
                index.Tables.Add(new BackupTableState
                {
                    TableName = table.TableName,
                    PrimaryKeyColumns = new List<string>(table.PrimaryKeyColumns),
                    TableHash = table.TableHash,
                    RowHashes = new Dictionary<string, string>(table.RowHashes, StringComparer.OrdinalIgnoreCase)
                });
            }

            foreach (var view in snapshot.Views)
            {
                index.Views.Add(new BackupViewState
                {
                    ViewName = view.ViewName,
                    DefinitionHash = ComputeSha256(view.CreateViewSql ?? string.Empty)
                });
            }

            return index;
        }

        private static BackupSnapshotIndex LoadSnapshotIndex(string snapshotPath)
        {
            if (string.IsNullOrWhiteSpace(snapshotPath) || !File.Exists(snapshotPath))
            {
                throw new FileNotFoundException("Backup snapshot metadata was not found.", snapshotPath);
            }

            var index = JsonConvert.DeserializeObject<BackupSnapshotIndex>(File.ReadAllText(snapshotPath));
            if (index == null)
            {
                throw new InvalidOperationException("Backup snapshot metadata is invalid.");
            }

            if (index.Tables == null)
            {
                index.Tables = new List<BackupTableState>();
            }

            if (index.Views == null)
            {
                index.Views = new List<BackupViewState>();
            }

            foreach (var table in index.Tables)
            {
                if (table.PrimaryKeyColumns == null)
                {
                    table.PrimaryKeyColumns = new List<string>();
                }

                if (table.RowHashes == null)
                {
                    table.RowHashes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
            }

            return index;
        }

        private static bool CanUseDeltaBackup(BackupSnapshotIndex currentIndex, BackupSnapshotIndex baselineIndex)
        {
            if (baselineIndex == null)
            {
                return false;
            }

            if (!string.Equals(currentIndex.DatabaseName, baselineIndex.DatabaseName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (currentIndex.Tables.Count != baselineIndex.Tables.Count || currentIndex.Views.Count != baselineIndex.Views.Count)
            {
                return false;
            }

            foreach (var currentTable in currentIndex.Tables)
            {
                var baselineTable = baselineIndex.Tables.FirstOrDefault(item => string.Equals(item.TableName, currentTable.TableName, StringComparison.OrdinalIgnoreCase));
                if (baselineTable == null)
                {
                    return false;
                }

                if (!currentTable.PrimaryKeyColumns.SequenceEqual(baselineTable.PrimaryKeyColumns ?? new List<string>(), StringComparer.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            foreach (var currentView in currentIndex.Views)
            {
                var baselineView = baselineIndex.Views.FirstOrDefault(item => string.Equals(item.ViewName, currentView.ViewName, StringComparison.OrdinalIgnoreCase));
                if (baselineView == null || !string.Equals(currentView.DefinitionHash, baselineView.DefinitionHash, StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }

        private static DeltaBackupPlan BuildDeltaPlan(DatabaseSnapshot currentSnapshot, BackupSnapshotIndex baselineIndex)
        {
            var plan = new DeltaBackupPlan();
            var baselineTables = baselineIndex.Tables.ToDictionary(item => item.TableName, StringComparer.OrdinalIgnoreCase);

            foreach (var currentTable in currentSnapshot.Tables)
            {
                BackupTableState baselineTable;
                baselineTables.TryGetValue(currentTable.TableName, out baselineTable);

                var delta = new DeltaTablePlan
                {
                    TableName = currentTable.TableName
                };
                delta.Columns.AddRange(currentTable.Columns);
                delta.PrimaryKeyColumns.AddRange(currentTable.PrimaryKeyColumns);

                var supportsRowDelta = currentTable.PrimaryKeyColumns.Count > 0
                    && baselineTable != null
                    && baselineTable.PrimaryKeyColumns != null
                    && baselineTable.PrimaryKeyColumns.SequenceEqual(currentTable.PrimaryKeyColumns, StringComparer.OrdinalIgnoreCase);

                if (!supportsRowDelta)
                {
                    if (baselineTable == null || !string.Equals(currentTable.TableHash, baselineTable.TableHash, StringComparison.Ordinal))
                    {
                        delta.ReplaceTable = true;
                        delta.Rows.AddRange(currentTable.Rows);
                    }
                }
                else
                {
                    foreach (var row in currentTable.Rows)
                    {
                        string previousHash;
                        if (!baselineTable.RowHashes.TryGetValue(row.PrimaryKeySignature, out previousHash)
                            || !string.Equals(previousHash, row.Hash, StringComparison.Ordinal))
                        {
                            delta.Rows.Add(row);
                        }
                    }

                    foreach (var deletedKey in baselineTable.RowHashes.Keys)
                    {
                        if (!currentTable.RowHashes.ContainsKey(deletedKey))
                        {
                            delta.DeletedKeys.Add(ParseKeySignature(deletedKey));
                        }
                    }
                }

                if (delta.HasChanges)
                {
                    plan.Tables.Add(delta);
                }
            }

            foreach (var baselineTable in baselineIndex.Tables)
            {
                var stillExists = currentSnapshot.Tables.Any(item => string.Equals(item.TableName, baselineTable.TableName, StringComparison.OrdinalIgnoreCase));
                if (!stillExists)
                {
                    plan.Tables.Add(new DeltaTablePlan
                    {
                        TableName = baselineTable.TableName,
                        ReplaceTable = true,
                        PrimaryKeyColumns = baselineTable.PrimaryKeyColumns ?? new List<string>()
                    });
                }
            }

            return plan;
        }

        private static IList<string> GetBaseTables(MySqlConnection connection)
        {
            const string sql = @"
SELECT TABLE_NAME
FROM information_schema.tables
WHERE TABLE_SCHEMA = @DatabaseName
  AND TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;";

            var tables = new List<string>();
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@DatabaseName", connection.Database);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tables.Add(reader.GetString(0));
                    }
                }
            }

            return tables;
        }

        private static IList<string> GetViews(MySqlConnection connection)
        {
            const string sql = @"
SELECT TABLE_NAME
FROM information_schema.views
WHERE TABLE_SCHEMA = @DatabaseName
ORDER BY TABLE_NAME;";

            var views = new List<string>();
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@DatabaseName", connection.Database);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        views.Add(reader.GetString(0));
                    }
                }
            }

            return views;
        }

        private static List<string> GetPrimaryKeyColumns(MySqlConnection connection, string tableName)
        {
            const string sql = @"
SELECT kcu.COLUMN_NAME
FROM information_schema.table_constraints tc
INNER JOIN information_schema.key_column_usage kcu
    ON tc.CONSTRAINT_NAME = kcu.CONSTRAINT_NAME
   AND tc.TABLE_SCHEMA = kcu.TABLE_SCHEMA
   AND tc.TABLE_NAME = kcu.TABLE_NAME
WHERE tc.TABLE_SCHEMA = @DatabaseName
  AND tc.TABLE_NAME = @TableName
  AND tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
ORDER BY kcu.ORDINAL_POSITION;";

            var columns = new List<string>();
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@DatabaseName", connection.Database);
                command.Parameters.AddWithValue("@TableName", tableName);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        columns.Add(reader.GetString(0));
                    }
                }
            }

            return columns;
        }

        private static List<string> GetTableColumns(MySqlConnection connection, string tableName)
        {
            const string sql = @"
SELECT COLUMN_NAME
FROM information_schema.columns
WHERE TABLE_SCHEMA = @DatabaseName
  AND TABLE_NAME = @TableName
ORDER BY ORDINAL_POSITION;";

            var columns = new List<string>();
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@DatabaseName", connection.Database);
                command.Parameters.AddWithValue("@TableName", tableName);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        columns.Add(reader.GetString(0));
                    }
                }
            }

            return columns;
        }

        private static string GetCreateTableSql(MySqlConnection connection, string tableName)
        {
            using (var command = new MySqlCommand("SHOW CREATE TABLE " + QuoteIdentifier(tableName) + ";", connection))
            using (var reader = command.ExecuteReader())
            {
                if (!reader.Read())
                {
                    throw new InvalidOperationException("Unable to read CREATE TABLE script for " + tableName + ".");
                }

                return EnsureSqlTerminator(reader.GetString(1));
            }
        }

        private static string GetCreateViewSql(MySqlConnection connection, string viewName)
        {
            using (var command = new MySqlCommand("SHOW CREATE VIEW " + QuoteIdentifier(viewName) + ";", connection))
            using (var reader = command.ExecuteReader())
            {
                if (!reader.Read())
                {
                    throw new InvalidOperationException("Unable to read CREATE VIEW script for " + viewName + ".");
                }

                return EnsureSqlTerminator(reader.GetString(1));
            }
        }

        private static string BuildFullBackupSql(DatabaseSnapshot snapshot)
        {
            var sql = new StringBuilder();
            AppendHeader(sql, DatabaseBackupKind.Full);
            sql.AppendLine("SET FOREIGN_KEY_CHECKS = 0;");
            sql.AppendLine();

            foreach (var view in snapshot.Views)
            {
                sql.Append("DROP VIEW IF EXISTS ");
                sql.Append(QuoteIdentifier(view.ViewName));
                sql.AppendLine(";");
            }

            if (snapshot.Views.Count > 0)
            {
                sql.AppendLine();
            }

            foreach (var table in snapshot.Tables)
            {
                sql.Append("DROP TABLE IF EXISTS ");
                sql.Append(QuoteIdentifier(table.TableName));
                sql.AppendLine(";");
            }

            sql.AppendLine();

            foreach (var table in snapshot.Tables)
            {
                sql.AppendLine(table.CreateTableSql);
                sql.AppendLine();
            }

            foreach (var table in snapshot.Tables)
            {
                AppendTableInsertStatements(sql, table.TableName, table.Columns, table.Rows);
            }

            if (snapshot.Views.Count > 0)
            {
                sql.AppendLine();
                foreach (var view in snapshot.Views)
                {
                    sql.AppendLine(view.CreateViewSql);
                    sql.AppendLine();
                }
            }

            sql.AppendLine("SET FOREIGN_KEY_CHECKS = 1;");
            return sql.ToString();
        }

        private static string BuildDeltaBackupSql(DeltaBackupPlan plan, DatabaseBackupKind backupKind)
        {
            var sql = new StringBuilder();
            AppendHeader(sql, backupKind);
            sql.AppendLine("SET FOREIGN_KEY_CHECKS = 0;");
            sql.AppendLine();

            if (plan.Tables.Count == 0)
            {
                sql.AppendLine("-- No row changes were detected for this backup.");
                sql.AppendLine("SET FOREIGN_KEY_CHECKS = 1;");
                return sql.ToString();
            }

            foreach (var table in plan.Tables)
            {
                if (table.ReplaceTable)
                {
                    sql.Append("DELETE FROM ");
                    sql.Append(QuoteIdentifier(table.TableName));
                    sql.AppendLine(";");
                    AppendTableInsertStatements(sql, table.TableName, table.Columns, table.Rows);
                    continue;
                }

                foreach (var deletedKey in table.DeletedKeys)
                {
                    sql.Append("DELETE FROM ");
                    sql.Append(QuoteIdentifier(table.TableName));
                    sql.Append(" WHERE ");
                    sql.Append(BuildWhereClause(deletedKey));
                    sql.AppendLine(";");
                }

                foreach (var row in table.Rows)
                {
                    sql.Append("DELETE FROM ");
                    sql.Append(QuoteIdentifier(table.TableName));
                    sql.Append(" WHERE ");
                    sql.Append(BuildWhereClause(ParseKeySignature(row.PrimaryKeySignature)));
                    sql.AppendLine(";");
                }

                AppendTableInsertStatements(sql, table.TableName, table.Columns, table.Rows);
            }

            sql.AppendLine("SET FOREIGN_KEY_CHECKS = 1;");
            return sql.ToString();
        }

        private static void AppendHeader(StringBuilder sql, DatabaseBackupKind backupKind)
        {
            sql.AppendLine("-- Hospital Management System Backup");
            sql.Append("-- Backup Type: ");
            sql.AppendLine(backupKind.ToString());
            sql.Append("-- Created UTC: ");
            sql.AppendLine(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            sql.AppendLine();
        }

        private static void AppendTableInsertStatements(
            StringBuilder sql,
            string tableName,
            IList<string> columns,
            IList<DatabaseTableRow> rows)
        {
            if (rows == null || rows.Count == 0 || columns == null || columns.Count == 0)
            {
                return;
            }

            sql.Append("-- Data for ");
            sql.AppendLine(tableName);
            foreach (var row in rows)
            {
                sql.Append("INSERT INTO ");
                sql.Append(QuoteIdentifier(tableName));
                sql.Append(" (");
                sql.Append(string.Join(", ", columns.Select(QuoteIdentifier)));
                sql.Append(") VALUES (");
                sql.Append(string.Join(", ", columns.Select(column => ToSqlLiteral(row.Values.ContainsKey(column) ? row.Values[column] : null))));
                sql.AppendLine(");");
            }

            sql.AppendLine();
        }

        private static string BuildWhereClause(IDictionary<string, string> keyValues)
        {
            return string.Join(
                " AND ",
                keyValues.Select(item => QuoteIdentifier(item.Key) + " = " + ToSqlLiteral(item.Value)));
        }

        private static string QuoteIdentifier(string identifier)
        {
            return "`" + (identifier ?? string.Empty).Replace("`", "``") + "`";
        }

        private static string ToSqlLiteral(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return "NULL";
            }

            if (value is byte[])
            {
                var bytes = (byte[])value;
                var builder = new StringBuilder(bytes.Length * 2 + 2);
                builder.Append("0x");
                foreach (var item in bytes)
                {
                    builder.Append(item.ToString("X2", CultureInfo.InvariantCulture));
                }

                return builder.ToString();
            }

            if (value is bool)
            {
                return (bool)value ? "1" : "0";
            }

            if (value is DateTime)
            {
                return "'" + ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture) + "'";
            }

            if (value is DateTimeOffset)
            {
                return "'" + ((DateTimeOffset)value).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture) + "'";
            }

            if (value is TimeSpan)
            {
                return "'" + ((TimeSpan)value).ToString() + "'";
            }

            if (value is string)
            {
                return "'" + MySqlHelper.EscapeString((string)value) + "'";
            }

            if (value is char)
            {
                return "'" + MySqlHelper.EscapeString(value.ToString()) + "'";
            }

            if (value is Guid)
            {
                return "'" + ((Guid)value).ToString("D") + "'";
            }

            if (value is sbyte || value is byte || value is short || value is ushort
                || value is int || value is uint || value is long || value is ulong
                || value is decimal || value is float || value is double)
            {
                return Convert.ToString(value, CultureInfo.InvariantCulture);
            }

            return "'" + MySqlHelper.EscapeString(Convert.ToString(value, CultureInfo.InvariantCulture)) + "'";
        }

        private static string ComputeRowHash(IDictionary<string, object> values, IList<string> columns)
        {
            var builder = new StringBuilder();
            foreach (var column in columns)
            {
                builder.Append(column);
                builder.Append('=');
                builder.Append(NormalizeHashValue(values.ContainsKey(column) ? values[column] : null));
                builder.Append(';');
            }

            return ComputeSha256(builder.ToString());
        }

        private static string ComputeTableHash(IEnumerable<string> rowHashes)
        {
            var content = string.Join("\n", rowHashes.OrderBy(item => item, StringComparer.Ordinal));
            return ComputeSha256(content);
        }

        private static string ComputeSha256(string value)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(value ?? string.Empty);
                var hash = sha256.ComputeHash(bytes);
                var builder = new StringBuilder(hash.Length * 2);
                foreach (var item in hash)
                {
                    builder.Append(item.ToString("x2", CultureInfo.InvariantCulture));
                }

                return builder.ToString();
            }
        }

        private static string NormalizeHashValue(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return "{null}";
            }

            if (value is byte[])
            {
                return "bytes:" + Convert.ToBase64String((byte[])value);
            }

            if (value is DateTime)
            {
                return "datetime:" + ((DateTime)value).ToString("o", CultureInfo.InvariantCulture);
            }

            if (value is DateTimeOffset)
            {
                return "datetimeoffset:" + ((DateTimeOffset)value).ToString("o", CultureInfo.InvariantCulture);
            }

            if (value is bool)
            {
                return "bool:" + ((bool)value ? "1" : "0");
            }

            if (value is TimeSpan)
            {
                return "timespan:" + ((TimeSpan)value).ToString();
            }

            if (value is decimal || value is double || value is float
                || value is sbyte || value is byte || value is short || value is ushort
                || value is int || value is uint || value is long || value is ulong)
            {
                return "number:" + Convert.ToString(value, CultureInfo.InvariantCulture);
            }

            return "text:" + Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private static string NormalizeKeyComponent(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            if (value is byte[])
            {
                return Convert.ToBase64String((byte[])value);
            }

            if (value is DateTime)
            {
                return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture);
            }

            if (value is DateTimeOffset)
            {
                return ((DateTimeOffset)value).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture);
            }

            if (value is bool)
            {
                return (bool)value ? "1" : "0";
            }

            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private static Dictionary<string, string> ParseKeySignature(string signature)
        {
            if (string.IsNullOrWhiteSpace(signature))
            {
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }

            var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(signature);
            return values ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        private static object CloneValue(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            if (value is byte[])
            {
                var original = (byte[])value;
                var copy = new byte[original.Length];
                Buffer.BlockCopy(original, 0, copy, 0, original.Length);
                return copy;
            }

            return value;
        }

        private static string EnsureSqlTerminator(string sql)
        {
            var trimmed = (sql ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                return string.Empty;
            }

            return trimmed.EndsWith(";", StringComparison.Ordinal) ? trimmed : trimmed + ";";
        }

        private static string SanitizeCreateViewSql(string createViewSql)
        {
            var cleaned = createViewSql ?? string.Empty;
            cleaned = Regex.Replace(cleaned, @"\s+DEFINER=`[^`]+`@`[^`]+`\s+", " ", RegexOptions.IgnoreCase);
            cleaned = cleaned.Replace("CREATE ALGORITHM", "CREATE OR REPLACE ALGORITHM");
            return EnsureSqlTerminator(cleaned);
        }

        private static string NormalizeBackupRoot(string backupRoot)
        {
            var root = string.IsNullOrWhiteSpace(backupRoot)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HospitalManagementSystem", "Backups")
                : backupRoot.Trim();
            return Path.GetFullPath(root);
        }

        private static void AttachBackupPaths(DatabaseBackupSetSummary manifest, string backupDirectory)
        {
            manifest.BackupDirectory = backupDirectory;
            manifest.ManifestPath = Path.Combine(backupDirectory, ManifestFileName);
            manifest.SqlScriptPath = Path.Combine(backupDirectory, SqlFileName);
            manifest.SnapshotPath = Path.Combine(backupDirectory, SnapshotFileName);
        }

        private static void WriteJson(string path, object value)
        {
            var json = JsonConvert.SerializeObject(value, Formatting.Indented);
            File.WriteAllText(path, json, Encoding.UTF8);
        }

        private string BuildSourceSignature()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}|{1}",
                (SourceServer ?? string.Empty).Trim().ToLowerInvariant(),
                (SourceDatabase ?? string.Empty).Trim().ToLowerInvariant());
        }

        private static string SanitizePathSegment(string value)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var builder = new StringBuilder();
            foreach (var ch in value ?? string.Empty)
            {
                builder.Append(invalid.Contains(ch) ? '_' : ch);
            }

            return builder.Length == 0 ? "backup" : builder.ToString();
        }

        private static List<DatabaseBackupSetSummary> ResolveRestoreChain(
            DatabaseBackupSetSummary target,
            IList<DatabaseBackupSetSummary> allBackups)
        {
            var byId = allBackups.ToDictionary(item => item.BackupId, StringComparer.OrdinalIgnoreCase);
            var chain = new List<DatabaseBackupSetSummary>();

            if (target.BackupKind == DatabaseBackupKind.Full)
            {
                chain.Add(target);
                return chain;
            }

            if (target.BackupKind == DatabaseBackupKind.Differential)
            {
                if (string.IsNullOrWhiteSpace(target.BaseFullBackupId))
                {
                    throw new InvalidOperationException("The selected differential backup does not reference a full backup.");
                }

                DatabaseBackupSetSummary fullBackup;
                if (!byId.TryGetValue(target.BaseFullBackupId, out fullBackup))
                {
                    throw new InvalidOperationException("The required full backup could not be found.");
                }

                chain.Add(fullBackup);
                chain.Add(target);
                return chain.OrderBy(item => item.CreatedAtUtc).ToList();
            }

            var cursor = target;
            while (cursor != null)
            {
                chain.Add(cursor);
                if (cursor.BackupKind == DatabaseBackupKind.Full)
                {
                    break;
                }

                if (cursor.BackupKind == DatabaseBackupKind.Differential)
                {
                    if (string.IsNullOrWhiteSpace(cursor.BaseFullBackupId))
                    {
                        throw new InvalidOperationException("The selected differential backup does not reference a full backup.");
                    }

                    DatabaseBackupSetSummary baseFull;
                    if (!byId.TryGetValue(cursor.BaseFullBackupId, out baseFull))
                    {
                        throw new InvalidOperationException("The required full backup could not be found.");
                    }

                    cursor = baseFull;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(cursor.PreviousBackupId))
                {
                    throw new InvalidOperationException("The selected incremental backup chain is incomplete.");
                }

                DatabaseBackupSetSummary previous;
                if (!byId.TryGetValue(cursor.PreviousBackupId, out previous))
                {
                    throw new InvalidOperationException("A required backup dependency is missing from the selected backup root.");
                }

                cursor = previous;
            }

            if (!chain.Any(item => item.BackupKind == DatabaseBackupKind.Full))
            {
                throw new InvalidOperationException("The selected incremental backup chain does not contain a full backup.");
            }

            return chain
                .Distinct()
                .OrderBy(item => item.CreatedAtUtc)
                .ToList();
        }

        private static void Report(IProgress<string> progress, string message)
        {
            if (progress != null && !string.IsNullOrWhiteSpace(message))
            {
                progress.Report(message);
            }
        }

        private sealed class DatabaseSnapshot
        {
            public DatabaseSnapshot()
            {
                Tables = new List<DatabaseTableSnapshot>();
                Views = new List<DatabaseViewSnapshot>();
            }

            public string DatabaseName { get; set; }

            public List<DatabaseTableSnapshot> Tables { get; private set; }

            public List<DatabaseViewSnapshot> Views { get; private set; }
        }

        private sealed class DatabaseTableSnapshot
        {
            public DatabaseTableSnapshot()
            {
                Columns = new List<string>();
                PrimaryKeyColumns = new List<string>();
                Rows = new List<DatabaseTableRow>();
                RowHashes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }

            public string TableName { get; set; }

            public string CreateTableSql { get; set; }

            public List<string> Columns { get; private set; }

            public List<string> PrimaryKeyColumns { get; private set; }

            public List<DatabaseTableRow> Rows { get; private set; }

            public Dictionary<string, string> RowHashes { get; private set; }

            public string TableHash { get; set; }
        }

        private sealed class DatabaseTableRow
        {
            public DatabaseTableRow()
            {
                Values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            }

            public Dictionary<string, object> Values { get; private set; }

            public string PrimaryKeySignature { get; set; }

            public string Hash { get; set; }
        }

        private sealed class DatabaseViewSnapshot
        {
            public string ViewName { get; set; }

            public string CreateViewSql { get; set; }
        }

        private sealed class DeltaBackupPlan
        {
            public DeltaBackupPlan()
            {
                Tables = new List<DeltaTablePlan>();
            }

            public List<DeltaTablePlan> Tables { get; private set; }
        }

        private sealed class DeltaTablePlan
        {
            public DeltaTablePlan()
            {
                Columns = new List<string>();
                PrimaryKeyColumns = new List<string>();
                Rows = new List<DatabaseTableRow>();
                DeletedKeys = new List<Dictionary<string, string>>();
            }

            public string TableName { get; set; }

            public List<string> Columns { get; set; }

            public List<string> PrimaryKeyColumns { get; set; }

            public bool ReplaceTable { get; set; }

            public List<DatabaseTableRow> Rows { get; private set; }

            public List<Dictionary<string, string>> DeletedKeys { get; private set; }

            public bool HasChanges => ReplaceTable || Rows.Count > 0 || DeletedKeys.Count > 0;
        }
    }
}
