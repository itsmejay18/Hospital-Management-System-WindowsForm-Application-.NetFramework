using System;
using MySql.Data.MySqlClient;

namespace HospitalManagementSystem.Helpers
{
    internal sealed class DatabaseConnectionProfile
    {
        public string Mode { get; set; } = DatabaseConnectionProfiles.OnlineMode;

        public string Transport { get; set; } = DatabaseConnectionProfiles.WiredTransport;

        public string Host { get; set; } = DatabaseDefaults.Server;

        public int Port { get; set; } = DatabaseDefaults.Port;

        public string DatabaseName { get; set; } = DatabaseDefaults.DatabaseName;

        public string Username { get; set; } = DatabaseDefaults.Username;

        public string Password { get; set; } = DatabaseDefaults.Password;

        public DatabaseConnectionProfile Clone()
        {
            return new DatabaseConnectionProfile
            {
                Mode = Mode,
                Transport = Transport,
                Host = Host,
                Port = Port,
                DatabaseName = DatabaseName,
                Username = Username,
                Password = Password
            };
        }

        public string BuildConnectionString()
        {
            return DatabaseConnectionProfiles.BuildConnectionString(Host, Port, DatabaseName, Username, Password);
        }
    }

    internal static class DatabaseConnectionProfiles
    {
        public const string LocalMode = "Local";
        public const string OnlineMode = "Online";
        public const string NetworkMode = "Network";
        public const string WiredTransport = "Wired";
        public const string WirelessTransport = "Wireless";

        private const string LocalHost = "localhost";
        private const int LocalPort = 3306;
        private const string LocalDatabase = "HospitalManagementSystem";
        private const string LocalUsername = "root";
        private const string LocalPassword = "root";

        private const string NetworkHost = "192.168.1.10";
        private const int NetworkPort = 3306;
        private const string NetworkDatabase = "HospitalManagementSystem";
        private const string NetworkUsername = "root";
        private const string NetworkPassword = "root";

        public static string NormalizeMode(string mode)
        {
            if (string.Equals(mode, LocalMode, StringComparison.OrdinalIgnoreCase))
            {
                return LocalMode;
            }

            if (string.Equals(mode, NetworkMode, StringComparison.OrdinalIgnoreCase))
            {
                return NetworkMode;
            }

            return OnlineMode;
        }

        public static string NormalizeTransport(string transport)
        {
            return string.Equals(transport, WirelessTransport, StringComparison.OrdinalIgnoreCase)
                ? WirelessTransport
                : WiredTransport;
        }

        public static DatabaseConnectionProfile CreatePreset(string mode)
        {
            switch (NormalizeMode(mode))
            {
                case LocalMode:
                    return new DatabaseConnectionProfile
                    {
                        Mode = LocalMode,
                        Transport = WiredTransport,
                        Host = LocalHost,
                        Port = LocalPort,
                        DatabaseName = LocalDatabase,
                        Username = LocalUsername,
                        Password = LocalPassword
                    };
                case NetworkMode:
                    return new DatabaseConnectionProfile
                    {
                        Mode = NetworkMode,
                        Transport = WiredTransport,
                        Host = NetworkHost,
                        Port = NetworkPort,
                        DatabaseName = NetworkDatabase,
                        Username = NetworkUsername,
                        Password = NetworkPassword
                    };
                default:
                    return new DatabaseConnectionProfile
                    {
                        Mode = OnlineMode,
                        Transport = WiredTransport,
                        Host = DatabaseDefaults.Server,
                        Port = DatabaseDefaults.Port,
                        DatabaseName = DatabaseDefaults.DatabaseName,
                        Username = DatabaseDefaults.Username,
                        Password = DatabaseDefaults.Password
                    };
            }
        }

        public static DatabaseConnectionProfile CreateFromAppSettings(AppSettingsProfile settings)
        {
            if (settings == null)
            {
                return CreatePreset(OnlineMode);
            }

            var mode = ResolveMode(settings.DatabaseMode, settings.DatabaseHost);
            var profile = CreatePreset(mode);
            profile.Transport = NormalizeTransport(settings.DatabaseTransport);

            if (!string.IsNullOrWhiteSpace(settings.DatabaseHost))
            {
                profile.Host = settings.DatabaseHost.Trim();
            }

            if (settings.DatabasePort > 0)
            {
                profile.Port = settings.DatabasePort;
            }

            if (!string.IsNullOrWhiteSpace(settings.DatabaseName))
            {
                profile.DatabaseName = settings.DatabaseName.Trim();
            }

            if (!string.IsNullOrWhiteSpace(settings.DatabaseUsername))
            {
                profile.Username = settings.DatabaseUsername.Trim();
            }

            profile.Password = settings.DatabasePassword ?? string.Empty;
            return profile;
        }

        public static void ApplyToAppSettings(AppSettingsProfile settings, DatabaseConnectionProfile profile)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            settings.DatabaseMode = NormalizeMode(profile.Mode);
            settings.DatabaseTransport = settings.DatabaseMode == NetworkMode
                ? NormalizeTransport(profile.Transport)
                : WiredTransport;
            settings.DatabaseHost = (profile.Host ?? string.Empty).Trim();
            settings.DatabasePort = profile.Port;
            settings.DatabaseName = (profile.DatabaseName ?? string.Empty).Trim();
            settings.DatabaseUsername = (profile.Username ?? string.Empty).Trim();
            settings.DatabasePassword = profile.Password ?? string.Empty;
            settings.DatabaseSetActiveProfile = true;
            settings.BootstrapConnection = profile.BuildConnectionString();
        }

        public static string BuildConnectionString(string host, int port, string database, string username, string password)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = string.IsNullOrWhiteSpace(host) ? DatabaseDefaults.Server : host.Trim(),
                Port = Convert.ToUInt32(port <= 0 ? DatabaseDefaults.Port : port),
                Database = string.IsNullOrWhiteSpace(database) ? DatabaseDefaults.DatabaseName : database.Trim(),
                UserID = string.IsNullOrWhiteSpace(username) ? DatabaseDefaults.Username : username.Trim(),
                Password = password ?? string.Empty,
                Pooling = true,
                CharacterSet = "utf8mb4",
                AllowPublicKeyRetrieval = true
            };

            return builder.ConnectionString;
        }

        private static string ResolveMode(string mode, string host)
        {
            if (string.Equals(mode, LocalMode, StringComparison.OrdinalIgnoreCase))
            {
                return LocalMode;
            }

            if (string.Equals(host?.Trim(), DatabaseDefaults.Server, StringComparison.OrdinalIgnoreCase))
            {
                return OnlineMode;
            }

            return NormalizeMode(mode);
        }
    }
}
