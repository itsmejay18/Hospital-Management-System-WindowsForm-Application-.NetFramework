using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using HospitalManagementSystem.DAL;
using HospitalManagementSystem.Helpers;
using MySql.Data.MySqlClient;

namespace HospitalManagementSystem
{
    public sealed class InstallationOptions
    {
        public string Server { get; set; } = "localhost";

        public int Port { get; set; } = 3306;

        public string DatabaseName { get; set; } = "HospitalManagementSystem";

        public string Username { get; set; } = "root";

        public string Password { get; set; } = "root";
    }

    public static class InstallationManager
    {
        private sealed class TableSchemaStatement
        {
            public string TableName { get; set; }

            public string Sql { get; set; }
        }

        public const string SuperAdminUsername = "superadmin";
        public const string DefaultSuperAdminPassword = "SuperAdmin123!";
        private const string ConnectionName = "HospitalDB";
        private const string SchemaFileName = "hospitalmanagementsystem.sql";

        public static bool CheckFirstRun()
        {
            if (!HasConnectionString())
            {
                return true;
            }

            if (!DatabaseExists())
            {
                return true;
            }

            return !SuperAdminExists();
        }

        public static bool HasConnectionString()
        {
            var connectionString = ResolvePersistedConnectionString();
            return !string.IsNullOrWhiteSpace(connectionString);
        }

        public static bool DatabaseExists()
        {
            try
            {
                var options = LoadFromPersistedConfiguration();
                var serverConnection = BuildConnectionString(options, includeDatabase: false);
                using (var connection = new MySqlConnection(serverConnection))
                {
                    connection.Open();
                    using (var command = new MySqlCommand(
                        "SELECT COUNT(*) FROM information_schema.schemata WHERE LOWER(schema_name) = LOWER(@DatabaseName);",
                        connection))
                    {
                        command.Parameters.AddWithValue("@DatabaseName", options.DatabaseName);
                        return Convert.ToInt32(command.ExecuteScalar(), CultureInfo.InvariantCulture) > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool SuperAdminExists()
        {
            try
            {
                var options = LoadFromPersistedConfiguration();
                var dbConnection = BuildConnectionString(options, includeDatabase: true);
                using (var connection = new MySqlConnection(dbConnection))
                {
                    connection.Open();
                    using (var command = new MySqlCommand(
                        @"SELECT COUNT(*)
                          FROM users u
                          INNER JOIN userroles r ON r.RoleID = u.RoleID
                          WHERE LOWER(u.Username) = LOWER(@Username)
                            AND LOWER(r.RoleName) = 'superadmin';",
                        connection))
                    {
                        command.Parameters.AddWithValue("@Username", SuperAdminUsername);
                        return Convert.ToInt32(command.ExecuteScalar(), CultureInfo.InvariantCulture) > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public static InstallationOptions LoadSuggestedOptions()
        {
            var options = LoadFromPersistedConfiguration();
            if (options.Port <= 0)
            {
                options.Port = 3306;
            }

            if (string.IsNullOrWhiteSpace(options.Server))
            {
                options.Server = "localhost";
            }

            if (string.IsNullOrWhiteSpace(options.DatabaseName))
            {
                options.DatabaseName = "HospitalManagementSystem";
            }

            if (string.IsNullOrWhiteSpace(options.Username))
            {
                options.Username = "root";
            }

            return options;
        }

        public static bool TestConnection(InstallationOptions options, out string message)
        {
            try
            {
                ValidateOptions(options);
                var connectionString = BuildConnectionString(options, includeDatabase: false);
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                }

                message = "Connection successful.";
                return true;
            }
            catch (Exception ex)
            {
                message = GetInnermostMessage(ex);
                return false;
            }
        }

        public static void Install(InstallationOptions options, string superAdminPassword)
        {
            ValidateOptions(options);
            if (string.IsNullOrWhiteSpace(superAdminPassword))
            {
                throw new InvalidOperationException("SuperAdmin password is required.");
            }

            var serverConnection = BuildConnectionString(options, includeDatabase: false);
            using (var connection = new MySqlConnection(serverConnection))
            {
                connection.Open();
                using (var command = new MySqlCommand(
                    $"CREATE DATABASE IF NOT EXISTS `{options.DatabaseName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;",
                    connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            var databaseConnection = BuildConnectionString(options, includeDatabase: true);
            using (var connection = new MySqlConnection(databaseConnection))
            {
                connection.Open();
                InstallFullSchema(connection);
                EnsureImageColumns(connection);
                EnsureRoles(connection);
                EnsureSuperAdmin(connection, superAdminPassword);
            }

            SaveConnectionString(databaseConnection);
            DatabaseConnection.SetRuntimeConnectionString(databaseConnection);
            SaveProfile(options, databaseConnection);
        }

        private static void InstallFullSchema(MySqlConnection connection)
        {
            var scriptPath = ResolveSchemaFilePath();
            if (string.IsNullOrWhiteSpace(scriptPath) || !File.Exists(scriptPath))
            {
                throw new FileNotFoundException(
                    "Full schema file was not found. Expected hospitalmanagementsystem.sql in app output.",
                    scriptPath);
            }

            var scriptText = File.ReadAllText(scriptPath);
            var statements = ExtractCreateTableStatements(scriptText);
            if (statements.Count == 0)
            {
                throw new InvalidOperationException("No CREATE TABLE statements were found in schema file.");
            }

            ExecuteSchemaStatements(connection, statements);
        }

        private static void EnsureImageColumns(MySqlConnection connection)
        {
            ExecuteNonQuerySafe(connection, "ALTER TABLE patients ADD COLUMN IF NOT EXISTS ProfileImage LONGBLOB NULL;");
            ExecuteNonQuerySafe(connection, "ALTER TABLE userdetails ADD COLUMN IF NOT EXISTS ProfileImage LONGBLOB NULL;");
            ExecuteNonQuerySafe(connection, "ALTER TABLE userdetails MODIFY COLUMN ProfileImage LONGBLOB NULL;");
        }

        private static void EnsureRoles(MySqlConnection connection)
        {
            var roles = new[]
            {
                Tuple.Create("Administrator", "Full system access"),
                Tuple.Create("SuperAdmin", "Installation super administrator"),
                Tuple.Create("Doctor", "Medical staff"),
                Tuple.Create("Nurse", "Nursing staff"),
                Tuple.Create("Receptionist", "Front desk"),
                Tuple.Create("Pharmacist", "Pharmacy management"),
                Tuple.Create("Lab Technician", "Laboratory test management"),
                Tuple.Create("Accountant", "Billing and finance"),
                Tuple.Create("HR Manager", "Human resources")
            };

            foreach (var role in roles)
            {
                using (var command = new MySqlCommand(
                    @"INSERT INTO userroles (RoleName, Description)
                      SELECT @RoleName, @Description
                      WHERE NOT EXISTS (
                          SELECT 1 FROM userroles WHERE LOWER(RoleName) = LOWER(@RoleName)
                      );",
                    connection))
                {
                    command.Parameters.AddWithValue("@RoleName", role.Item1);
                    command.Parameters.AddWithValue("@Description", role.Item2);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void EnsureSuperAdmin(MySqlConnection connection, string plainPassword)
        {
            int roleId;
            using (var roleCommand = new MySqlCommand(
                "SELECT RoleID FROM userroles WHERE LOWER(RoleName) = 'superadmin' LIMIT 1;",
                connection))
            {
                var roleValue = roleCommand.ExecuteScalar();
                if (roleValue == null || roleValue == DBNull.Value)
                {
                    throw new InvalidOperationException("SuperAdmin role could not be created.");
                }

                roleId = Convert.ToInt32(roleValue, CultureInfo.InvariantCulture);
            }

            var passwordHash = ComputeSha256(plainPassword);
            var email = "superadmin@hospital.local";

            using (var updateCommand = new MySqlCommand(
                @"UPDATE users
                  SET PasswordHash = @PasswordHash,
                      Email = @Email,
                      RoleID = @RoleID,
                      IsActive = 1
                  WHERE LOWER(Username) = LOWER(@Username);",
                connection))
            {
                updateCommand.Parameters.AddWithValue("@PasswordHash", passwordHash);
                updateCommand.Parameters.AddWithValue("@Email", email);
                updateCommand.Parameters.AddWithValue("@RoleID", roleId);
                updateCommand.Parameters.AddWithValue("@Username", SuperAdminUsername);

                var updated = updateCommand.ExecuteNonQuery();
                if (updated > 0)
                {
                    return;
                }
            }

            using (var insertCommand = new MySqlCommand(
                @"INSERT INTO users
                  (Username, PasswordHash, Email, RoleID, IsActive, LastLogin, CreatedDate)
                  VALUES
                  (@Username, @PasswordHash, @Email, @RoleID, 1, NULL, NOW());",
                connection))
            {
                insertCommand.Parameters.AddWithValue("@Username", SuperAdminUsername);
                insertCommand.Parameters.AddWithValue("@PasswordHash", passwordHash);
                insertCommand.Parameters.AddWithValue("@Email", email);
                insertCommand.Parameters.AddWithValue("@RoleID", roleId);
                insertCommand.ExecuteNonQuery();
            }
        }

        private static string ComputeSha256(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input ?? string.Empty);
                var hash = sha256.ComputeHash(bytes);
                var builder = new StringBuilder(hash.Length * 2);
                for (var i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("x2", CultureInfo.InvariantCulture));
                }

                return builder.ToString();
            }
        }

        private static void ExecuteSchemaStatements(MySqlConnection connection, IList<TableSchemaStatement> statements)
        {
            using (var disable = new MySqlCommand("SET FOREIGN_KEY_CHECKS = 0;", connection))
            {
                disable.ExecuteNonQuery();
            }

            try
            {
                var pending = new List<TableSchemaStatement>(statements);
                var failures = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                var pass = 0;
                while (pending.Count > 0 && pass < 8)
                {
                    pass++;
                    var remaining = new List<TableSchemaStatement>();
                    var executed = 0;

                    foreach (var statement in pending)
                    {
                        if (TableExists(connection, statement.TableName))
                        {
                            continue;
                        }

                        if (TryExecuteStatementWithCompatibility(connection, statement, out var error))
                        {
                            executed++;
                            failures.Remove(statement.TableName);
                            continue;
                        }

                        failures[statement.TableName] = error;
                        remaining.Add(statement);
                    }

                    if (remaining.Count == 0)
                    {
                        break;
                    }

                    if (executed == 0)
                    {
                        break;
                    }

                    pending = remaining;
                }

                var unresolved = new List<string>();
                foreach (var item in pending)
                {
                    if (!TableExists(connection, item.TableName))
                    {
                        unresolved.Add(item.TableName);
                    }
                }

                if (unresolved.Count > 0)
                {
                    EnsureCoreSchema(connection);

                    var requiredCore = new[] { "userroles", "users" };
                    var missingCore = new List<string>();
                    foreach (var coreTable in requiredCore)
                    {
                        if (!TableExists(connection, coreTable))
                        {
                            missingCore.Add(coreTable);
                        }
                    }

                    if (missingCore.Count > 0)
                    {
                        throw new InvalidOperationException(
                            "Schema installation failed. Missing core tables: " + string.Join(", ", missingCore));
                    }
                }
            }
            finally
            {
                using (var enable = new MySqlCommand("SET FOREIGN_KEY_CHECKS = 1;", connection))
                {
                    enable.ExecuteNonQuery();
                }
            }
        }

        private static bool TryExecuteStatementWithCompatibility(
            MySqlConnection connection,
            TableSchemaStatement statement,
            out string errorMessage)
        {
            if (statement == null || string.IsNullOrWhiteSpace(statement.Sql))
            {
                errorMessage = "Empty statement.";
                return false;
            }

            if (TryExecuteSql(connection, statement.Sql, out errorMessage))
            {
                return true;
            }

            foreach (var fallbackSql in BuildCompatibilityStatements(statement.Sql))
            {
                if (TryExecuteSql(connection, fallbackSql, out errorMessage))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool TryExecuteSql(MySqlConnection connection, string sql, out string errorMessage)
        {
            try
            {
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }

                errorMessage = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = GetInnermostMessage(ex);
                return false;
            }
        }

        private static IEnumerable<string> BuildCompatibilityStatements(string sql)
        {
            var candidate1 = Regex.Replace(
                sql,
                "utf8mb4_0900_ai_ci",
                "utf8mb4_general_ci",
                RegexOptions.IgnoreCase);
            if (!string.Equals(candidate1, sql, StringComparison.Ordinal))
            {
                yield return candidate1;
            }

            var source = string.Equals(candidate1, sql, StringComparison.Ordinal) ? sql : candidate1;
            var candidate2 = Regex.Replace(
                source,
                @"\s+COLLATE\s*=\s*utf8mb4_general_ci",
                string.Empty,
                RegexOptions.IgnoreCase);
            if (!string.Equals(candidate2, source, StringComparison.Ordinal))
            {
                yield return candidate2;
            }

            var candidate3 = Regex.Replace(
                candidate2,
                @"\s+COLLATE\s*=\s*utf8mb4_0900_ai_ci",
                string.Empty,
                RegexOptions.IgnoreCase);
            if (!string.Equals(candidate3, candidate2, StringComparison.Ordinal))
            {
                yield return candidate3;
            }
        }

        private static bool TableExists(MySqlConnection connection, string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                return false;
            }

            using (var command = new MySqlCommand(
                @"SELECT COUNT(*)
                  FROM information_schema.tables
                  WHERE table_schema = DATABASE()
                    AND LOWER(table_name) = LOWER(@TableName);",
                connection))
            {
                command.Parameters.AddWithValue("@TableName", tableName.Trim());
                return Convert.ToInt32(command.ExecuteScalar(), CultureInfo.InvariantCulture) > 0;
            }
        }

        private static void EnsureCoreSchema(MySqlConnection connection)
        {
            ExecuteNonQuerySafe(
                connection,
                @"CREATE TABLE IF NOT EXISTS userroles (
                    RoleID INT NOT NULL AUTO_INCREMENT,
                    RoleName VARCHAR(50) NOT NULL,
                    Description VARCHAR(255) NULL,
                    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (RoleID),
                    UNIQUE KEY UX_userroles_rolename (RoleName)
                  ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;");

            ExecuteNonQuerySafe(
                connection,
                @"CREATE TABLE IF NOT EXISTS users (
                    UserID INT NOT NULL AUTO_INCREMENT,
                    Username VARCHAR(50) NOT NULL,
                    PasswordHash VARCHAR(255) NOT NULL,
                    Email VARCHAR(100) NULL,
                    RoleID INT NOT NULL,
                    IsActive TINYINT(1) DEFAULT 1,
                    LastLogin DATETIME NULL,
                    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (UserID),
                    UNIQUE KEY UX_users_username (Username),
                    UNIQUE KEY UX_users_email (Email),
                    KEY idx_users_role (RoleID, IsActive),
                    CONSTRAINT FK_users_roleid FOREIGN KEY (RoleID) REFERENCES userroles(RoleID) ON DELETE RESTRICT
                  ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;");
        }

        private static List<TableSchemaStatement> ExtractCreateTableStatements(string scriptText)
        {
            var results = new List<TableSchemaStatement>();
            if (string.IsNullOrWhiteSpace(scriptText))
            {
                return results;
            }

            var builder = new StringBuilder();
            var inCreateTable = false;
            var lines = scriptText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                var trimmed = (line ?? string.Empty).Trim();
                if (trimmed.Length == 0)
                {
                    continue;
                }

                if (trimmed.StartsWith("--", StringComparison.Ordinal))
                {
                    continue;
                }

                if (trimmed.StartsWith("/*!", StringComparison.Ordinal))
                {
                    continue;
                }

                if (!inCreateTable)
                {
                    if (!trimmed.StartsWith("CREATE TABLE", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    inCreateTable = true;
                    builder.Clear();
                }

                builder.AppendLine(line);
                if (!trimmed.EndsWith(";", StringComparison.Ordinal))
                {
                    continue;
                }

                inCreateTable = false;
                var statement = builder.ToString().Trim();
                if (statement.Length == 0)
                {
                    continue;
                }

                statement = Regex.Replace(
                    statement,
                    @"^\s*CREATE\s+TABLE\s+",
                    "CREATE TABLE IF NOT EXISTS ",
                    RegexOptions.IgnoreCase);

                var tableName = ExtractTableName(statement);
                if (!string.IsNullOrWhiteSpace(tableName))
                {
                    results.Add(new TableSchemaStatement
                    {
                        TableName = tableName,
                        Sql = statement
                    });
                }

                builder.Clear();
            }

            return results;
        }

        private static string ExtractTableName(string createTableStatement)
        {
            if (string.IsNullOrWhiteSpace(createTableStatement))
            {
                return string.Empty;
            }

            var match = Regex.Match(
                createTableStatement,
                @"CREATE\s+TABLE\s+(?:IF\s+NOT\s+EXISTS\s+)?`?(?<name>[A-Za-z0-9_]+)`?",
                RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                return string.Empty;
            }

            return match.Groups["name"].Value;
        }

        private static void ExecuteNonQuerySafe(MySqlConnection connection, string sql)
        {
            try
            {
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                // Best effort for compatibility across schema versions.
            }
        }

        private static string ResolveSchemaFilePath()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var candidates = new[]
            {
                Path.Combine(baseDirectory, "Schema", SchemaFileName),
                Path.Combine(baseDirectory, SchemaFileName),
                Path.Combine(baseDirectory, "Dumps", SchemaFileName),
                Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", "Dumps", SchemaFileName)),
                Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", "..", "Dumps", SchemaFileName))
            };

            foreach (var path in candidates)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            return string.Empty;
        }

        private static void ValidateOptions(InstallationOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (string.IsNullOrWhiteSpace(options.Server))
            {
                throw new InvalidOperationException("Server is required.");
            }

            if (options.Port <= 0)
            {
                throw new InvalidOperationException("Port must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(options.DatabaseName))
            {
                throw new InvalidOperationException("Database name is required.");
            }

            if (string.IsNullOrWhiteSpace(options.Username))
            {
                throw new InvalidOperationException("Database username is required.");
            }
        }

        private static string BuildConnectionString(InstallationOptions options, bool includeDatabase)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = options.Server.Trim(),
                Port = Convert.ToUInt32(options.Port, CultureInfo.InvariantCulture),
                UserID = options.Username.Trim(),
                Password = options.Password ?? string.Empty,
                Pooling = true,
                CharacterSet = "utf8mb4",
                AllowPublicKeyRetrieval = true
            };

            if (includeDatabase)
            {
                builder.Database = options.DatabaseName.Trim();
            }

            return builder.ConnectionString;
        }

        private static string ResolvePersistedConnectionString()
        {
            var profileConnection = AppSettingsStore.Load()?.BootstrapConnection;
            if (!string.IsNullOrWhiteSpace(profileConnection))
            {
                return profileConnection.Trim();
            }

            var configConnection = ConfigurationManager.ConnectionStrings[ConnectionName]?.ConnectionString;
            return string.IsNullOrWhiteSpace(configConnection) ? string.Empty : configConnection.Trim();
        }

        private static InstallationOptions LoadFromPersistedConfiguration()
        {
            var options = new InstallationOptions();
            var connectionString = ResolvePersistedConnectionString();
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return options;
            }

            var builder = new MySqlConnectionStringBuilder(connectionString);
            options.Server = string.IsNullOrWhiteSpace(builder.Server) ? "localhost" : builder.Server;
            options.Port = builder.Port > 0 ? Convert.ToInt32(builder.Port, CultureInfo.InvariantCulture) : 3306;
            options.DatabaseName = string.IsNullOrWhiteSpace(builder.Database) ? "HospitalManagementSystem" : builder.Database;
            options.Username = string.IsNullOrWhiteSpace(builder.UserID) ? "root" : builder.UserID;
            options.Password = builder.Password ?? string.Empty;
            return options;
        }

        private static void SaveConnectionString(string connectionString)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var current = config.ConnectionStrings.ConnectionStrings[ConnectionName];
            if (current == null)
            {
                config.ConnectionStrings.ConnectionStrings.Add(
                    new ConnectionStringSettings(ConnectionName, connectionString, "MySql.Data.MySqlClient"));
            }
            else
            {
                current.ConnectionString = connectionString;
                current.ProviderName = "MySql.Data.MySqlClient";
            }

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("connectionStrings");
        }

        private static void SaveProfile(InstallationOptions options, string connectionString)
        {
            try
            {
                var profile = AppSettingsStore.Load();
                profile.DatabaseMode = IsLocalHost(options.Server) ? "Local" : "Network";
                profile.DatabaseTransport = profile.DatabaseMode == "Local" ? "Wired" : "Wireless";
                profile.DatabaseHost = options.Server;
                profile.DatabasePort = options.Port;
                profile.DatabaseName = options.DatabaseName;
                profile.DatabaseUsername = options.Username;
                profile.DatabasePassword = options.Password ?? string.Empty;
                profile.DatabaseSetActiveProfile = true;
                profile.BootstrapConnection = connectionString;
                AppSettingsStore.Save(profile);
            }
            catch
            {
                // Connection string already persisted in config; ignore profile write issues.
            }
        }

        private static bool IsLocalHost(string host)
        {
            var value = (host ?? string.Empty).Trim();
            return string.Equals(value, "localhost", StringComparison.OrdinalIgnoreCase)
                   || string.Equals(value, "127.0.0.1", StringComparison.OrdinalIgnoreCase)
                   || string.Equals(value, ".", StringComparison.OrdinalIgnoreCase);
        }

        private static string GetInnermostMessage(Exception exception)
        {
            if (exception == null)
            {
                return "Unknown error.";
            }

            var current = exception;
            while (current.InnerException != null)
            {
                current = current.InnerException;
            }

            return current.Message;
        }
    }
}
