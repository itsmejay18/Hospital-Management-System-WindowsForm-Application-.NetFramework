using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;

namespace HospitalManagementSystem.Helpers
{
    public enum DatabaseBackupKind
    {
        Full = 0,
        Incremental = 1,
        Differential = 2
    }

    public sealed class DatabaseBackupSetSummary
    {
        public string BackupId { get; set; }

        public DatabaseBackupKind BackupKind { get; set; }

        public string SourceServer { get; set; }

        public string SourceDatabase { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public string BaseFullBackupId { get; set; }

        public string PreviousBackupId { get; set; }

        public int TotalTables { get; set; }

        public int TotalRows { get; set; }

        public int ChangedTables { get; set; }

        public int ChangedRows { get; set; }

        public bool IsEmptyDelta { get; set; }

        [JsonIgnore]
        public string BackupDirectory { get; set; }

        [JsonIgnore]
        public string ManifestPath { get; set; }

        [JsonIgnore]
        public string SqlScriptPath { get; set; }

        [JsonIgnore]
        public string SnapshotPath { get; set; }

        [JsonIgnore]
        public string BackupDisplayName
        {
            get
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    "{0:yyyy-MM-dd HH:mm:ss} {1}",
                    CreatedAtUtc.ToLocalTime(),
                    BackupKind);
            }
        }

        [JsonIgnore]
        public DateTime CreatedAtLocal => CreatedAtUtc.ToLocalTime();

        [JsonIgnore]
        public string DependencyText
        {
            get
            {
                if (BackupKind == DatabaseBackupKind.Full)
                {
                    return "Self-contained";
                }

                if (BackupKind == DatabaseBackupKind.Differential)
                {
                    return string.IsNullOrWhiteSpace(BaseFullBackupId) ? "Needs full backup" : "Based on full backup";
                }

                return string.IsNullOrWhiteSpace(PreviousBackupId) ? "Needs previous backup" : "Based on latest backup";
            }
        }

        [JsonIgnore]
        public string SourceSignature
        {
            get
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}|{1}",
                    (SourceServer ?? string.Empty).Trim().ToLowerInvariant(),
                    (SourceDatabase ?? string.Empty).Trim().ToLowerInvariant());
            }
        }
    }

    internal sealed class BackupSnapshotIndex
    {
        public BackupSnapshotIndex()
        {
            Tables = new List<BackupTableState>();
            Views = new List<BackupViewState>();
        }

        public string DatabaseName { get; set; }

        public List<BackupTableState> Tables { get; set; }

        public List<BackupViewState> Views { get; set; }
    }

    internal sealed class BackupTableState
    {
        public BackupTableState()
        {
            PrimaryKeyColumns = new List<string>();
            RowHashes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public string TableName { get; set; }

        public List<string> PrimaryKeyColumns { get; set; }

        public string TableHash { get; set; }

        public Dictionary<string, string> RowHashes { get; set; }
    }

    internal sealed class BackupViewState
    {
        public string ViewName { get; set; }

        public string DefinitionHash { get; set; }
    }
}
