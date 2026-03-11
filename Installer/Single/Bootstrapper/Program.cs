using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;

namespace HospitalManagementSystemSetup
{
    internal static class Program
    {
        private const string ProductName = "Hospital Management System";
        private const string AppFolderName = "Hospital Management System";
        private const string ExecutableName = "Hospital Management System.exe";
        private const string PayloadResourceName = "HospitalManagementSystemSetup.Payload.AppPayload.zip";
        private const string HostingerConnectionString =
            "server=srv1237.hstgr.io;port=3306;database=u621755393_hospitalmanage;user id=u621755393_hospitalmanage;password=Dssc@2026;pooling=True;charset=utf8mb4;AllowPublicKeyRetrieval=True";

        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                var installDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Programs",
                    AppFolderName);

                Directory.CreateDirectory(installDir);
                ExtractPayload(installDir);
                EnsureSettingsProfile();
                CreateShortcuts(installDir);

                MessageBox.Show(
                    $"{ProductName} has been installed to:{Environment.NewLine}{installDir}",
                    ProductName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                StartApplication(installDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Installation failed:{Environment.NewLine}{ex.Message}",
                    ProductName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static void ExtractPayload(string installDir)
        {
            var installRoot = Path.GetFullPath(installDir).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(PayloadResourceName))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException("Installer payload was not found.");
                }

                using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                {
                    foreach (var entry in archive.Entries)
                    {
                        var destinationPath = Path.GetFullPath(Path.Combine(installDir, entry.FullName));
                        if (!destinationPath.StartsWith(installRoot, StringComparison.OrdinalIgnoreCase))
                        {
                            throw new InvalidOperationException("Installer payload contains an invalid path.");
                        }

                        if (string.IsNullOrEmpty(entry.Name))
                        {
                            Directory.CreateDirectory(destinationPath);
                            continue;
                        }

                        var destinationDirectory = Path.GetDirectoryName(destinationPath);
                        if (!string.IsNullOrWhiteSpace(destinationDirectory))
                        {
                            Directory.CreateDirectory(destinationDirectory);
                        }

                        entry.ExtractToFile(destinationPath, true);
                    }
                }
            }
        }

        private static void EnsureSettingsProfile()
        {
            var settingsDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "HospitalManagementSystem");
            var settingsPath = Path.Combine(settingsDir, "system-settings.xml");

            Directory.CreateDirectory(settingsDir);

            var document = new XmlDocument();
            if (File.Exists(settingsPath))
            {
                try
                {
                    document.Load(settingsPath);
                }
                catch
                {
                    document = CreateDefaultSettingsDocument();
                }
            }
            else
            {
                document = CreateDefaultSettingsDocument();
            }

            if (document.DocumentElement == null || !string.Equals(document.DocumentElement.Name, "AppSettingsProfile", StringComparison.Ordinal))
            {
                document = CreateDefaultSettingsDocument();
            }

            var profile = document.DocumentElement;
            SetNodeValue(document, profile, "DatabaseMode", "Online");
            SetNodeValue(document, profile, "DatabaseTransport", "Wired");
            SetNodeValue(document, profile, "DbProfileKey", string.Empty);
            SetNodeValue(document, profile, "DatabaseHost", "srv1237.hstgr.io");
            SetNodeValue(document, profile, "DatabasePort", "3306");
            SetNodeValue(document, profile, "DatabaseName", "u621755393_hospitalmanage");
            SetNodeValue(document, profile, "DatabaseUsername", "u621755393_hospitalmanage");
            SetNodeValue(document, profile, "DatabasePassword", "Dssc@2026");
            SetNodeValue(document, profile, "DatabaseSetActiveProfile", "true");
            SetNodeValue(document, profile, "BootstrapConnection", HostingerConnectionString);

            document.Save(settingsPath);
        }

        private static XmlDocument CreateDefaultSettingsDocument()
        {
            var document = new XmlDocument();
            var declaration = document.CreateXmlDeclaration("1.0", "utf-8", null);
            document.AppendChild(declaration);

            var root = document.CreateElement("AppSettingsProfile");
            root.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            root.SetAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
            document.AppendChild(root);

            AppendNode(document, root, "CompanyName", ProductName);
            AppendNode(document, root, "Address", "Main Street, City");
            AppendNode(document, root, "Phone", "+63 9XX XXX XXXX");
            AppendNode(document, root, "Email", "support@hospital.local");
            AppendNode(document, root, "DefaultDailyRate", "1000");
            AppendNode(document, root, "LateFeePerDay", "200");
            AppendNode(document, root, "TaxRatePercent", "12");
            AppendNode(document, root, "BackupPath", @"C:\HospitalBackups");
            AppendNode(document, root, "MySqlDumpPath", "mysqldump");
            AppendNode(document, root, "SmtpHost", "smtp.gmail.com");
            AppendNode(document, root, "SmtpPort", "587");
            AppendNode(document, root, "SmtpUser", string.Empty);
            AppendNode(document, root, "SmtpPassword", string.Empty);
            AppendNode(document, root, "EnableSsl", "true");
            AppendNode(document, root, "EnableDarkMode", "false");

            return document;
        }

        private static void AppendNode(XmlDocument document, XmlNode parent, string name, string value)
        {
            var node = document.CreateElement(name);
            node.InnerText = value ?? string.Empty;
            parent.AppendChild(node);
        }

        private static void SetNodeValue(XmlDocument document, XmlNode parent, string name, string value)
        {
            var node = parent.SelectSingleNode(name);
            if (node == null)
            {
                node = document.CreateElement(name);
                parent.AppendChild(node);
            }

            node.InnerText = value ?? string.Empty;
        }

        private static void CreateShortcuts(string installDir)
        {
            var executablePath = Path.Combine(installDir, ExecutableName);
            var iconPath = Path.Combine(installDir, "Assets", "branding-logo.ico");
            if (!File.Exists(executablePath) || !File.Exists(iconPath))
            {
                return;
            }

            CreateShortcut(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), $"{ProductName}.lnk"),
                executablePath,
                installDir,
                iconPath);

            CreateShortcut(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), $"{ProductName}.lnk"),
                executablePath,
                installDir,
                iconPath);
        }

        private static void CreateShortcut(string shortcutPath, string targetPath, string workingDirectory, string iconPath)
        {
            object shell = null;
            object shortcut = null;

            try
            {
                var shellType = Type.GetTypeFromProgID("WScript.Shell");
                if (shellType == null)
                {
                    return;
                }

                shell = Activator.CreateInstance(shellType);
                shortcut = shellType.InvokeMember(
                    "CreateShortcut",
                    BindingFlags.InvokeMethod,
                    null,
                    shell,
                    new object[] { shortcutPath });

                var shortcutType = shortcut.GetType();
                shortcutType.InvokeMember("TargetPath", BindingFlags.SetProperty, null, shortcut, new object[] { targetPath });
                shortcutType.InvokeMember("WorkingDirectory", BindingFlags.SetProperty, null, shortcut, new object[] { workingDirectory });
                shortcutType.InvokeMember("IconLocation", BindingFlags.SetProperty, null, shortcut, new object[] { iconPath });
                shortcutType.InvokeMember("Description", BindingFlags.SetProperty, null, shortcut, new object[] { ProductName });
                shortcutType.InvokeMember("Save", BindingFlags.InvokeMethod, null, shortcut, null);
            }
            catch
            {
                // Shortcut creation is best-effort only.
            }
            finally
            {
                ReleaseComObject(shortcut);
                ReleaseComObject(shell);
            }
        }

        private static void ReleaseComObject(object instance)
        {
            if (instance != null && Marshal.IsComObject(instance))
            {
                Marshal.FinalReleaseComObject(instance);
            }
        }

        private static void StartApplication(string installDir)
        {
            var executablePath = Path.Combine(installDir, ExecutableName);
            if (!File.Exists(executablePath))
            {
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = executablePath,
                WorkingDirectory = installDir,
                UseShellExecute = true
            });
        }
    }
}
