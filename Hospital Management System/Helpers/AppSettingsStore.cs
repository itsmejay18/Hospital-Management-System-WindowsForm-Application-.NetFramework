using System;
using System.IO;
using System.Xml.Serialization;

namespace HospitalManagementSystem.Helpers
{
    /// <summary>
    /// Represents persisted system settings edited in the Settings module.
    /// </summary>
    public sealed class AppSettingsProfile
    {
        public string CompanyName { get; set; } = "Hospital Management System";

        public string Address { get; set; } = "Main Street, City";

        public string Phone { get; set; } = "+63 9XX XXX XXXX";

        public string Email { get; set; } = "support@hospital.local";

        public decimal DefaultDailyRate { get; set; } = 1000m;

        public decimal LateFeePerDay { get; set; } = 200m;

        public decimal TaxRatePercent { get; set; } = 12m;

        public string BackupPath { get; set; } = @"C:\HospitalBackups";

        public string MySqlDumpPath { get; set; } = "mysqldump";

        public string SmtpHost { get; set; } = "smtp.gmail.com";

        public int SmtpPort { get; set; } = 587;

        public string SmtpUser { get; set; } = string.Empty;

        public string SmtpPassword { get; set; } = string.Empty;

        public bool EnableSsl { get; set; } = true;

        public bool EnableDarkMode { get; set; }

        public string DatabaseMode { get; set; } = "Local";

        public string DatabaseTransport { get; set; } = "Wired";

        public string DbProfileKey { get; set; } = string.Empty;

        public string DatabaseHost { get; set; } = DatabaseDefaults.Server;

        public int DatabasePort { get; set; } = DatabaseDefaults.Port;

        public string DatabaseName { get; set; } = DatabaseDefaults.DatabaseName;

        public string DatabaseUsername { get; set; } = DatabaseDefaults.Username;

        public string DatabasePassword { get; set; } = DatabaseDefaults.Password;

        public bool DatabaseSetActiveProfile { get; set; } = true;

        public string BootstrapConnection { get; set; } = DatabaseDefaults.ConnectionString;
    }

    /// <summary>
    /// Handles loading and saving <see cref="AppSettingsProfile"/> to a local XML file.
    /// </summary>
    public static class AppSettingsStore
    {
        private const string SettingsFolderName = "HospitalManagementSystem";
        private const string SettingsFileName = "system-settings.xml";
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// Loads persisted settings if available, otherwise returns defaults.
        /// </summary>
        public static AppSettingsProfile Load()
        {
            lock (SyncRoot)
            {
                try
                {
                    var path = GetSettingsFilePath();
                    if (!File.Exists(path))
                    {
                        return new AppSettingsProfile();
                    }

                    var serializer = new XmlSerializer(typeof(AppSettingsProfile));
                    using (var stream = File.OpenRead(path))
                    {
                        if (serializer.Deserialize(stream) is AppSettingsProfile profile)
                        {
                            return profile;
                        }
                    }
                }
                catch
                {
                    // Fall back to defaults for corrupted or inaccessible settings files.
                }

                return new AppSettingsProfile();
            }
        }

        /// <summary>
        /// Persists settings to the local user profile.
        /// </summary>
        public static void Save(AppSettingsProfile profile)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            lock (SyncRoot)
            {
                var path = GetSettingsFilePath();
                var directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var serializer = new XmlSerializer(typeof(AppSettingsProfile));
                using (var stream = File.Create(path))
                {
                    serializer.Serialize(stream, profile);
                }
            }
        }

        private static string GetSettingsFilePath()
        {
            var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(basePath, SettingsFolderName, SettingsFileName);
        }
    }
}
