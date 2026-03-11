using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HospitalManagementSystem.Forms;
using HospitalManagementSystem.Helpers;

namespace HospitalManagementSystem
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (ShouldRunLocalInstaller())
            {
                using (var installer = new InstallerForm())
                {
                    if (installer.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                }
            }

            Application.Run(new frmLogin());
        }

        private static bool ShouldRunLocalInstaller()
        {
            try
            {
                var profile = AppSettingsStore.Load();
                if (profile != null
                    && string.Equals(
                        profile.DatabaseHost?.Trim(),
                        DatabaseDefaults.Server,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            catch
            {
                // Fall back to the existing first-run logic.
            }

            return InstallationManager.CheckFirstRun();
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleUnhandledException("UI Thread", e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleUnhandledException("AppDomain", e.ExceptionObject as Exception);
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            HandleUnhandledException("Background Task", e.Exception);
            e.SetObserved();
        }

        private static void HandleUnhandledException(string source, Exception exception)
        {
            var ex = exception ?? new Exception("Unknown application error.");
            try
            {
                var logDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "HospitalManagementSystem");
                Directory.CreateDirectory(logDirectory);

                var logFilePath = Path.Combine(logDirectory, "error.log");
                var entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {source}: {ex}\r\n";
                File.AppendAllText(logFilePath, entry);
            }
            catch
            {
                // Ignore logging failures.
            }

            Trace.TraceError("{0}: {1}", source, ex);
            MessageBox.Show(
                $"An unexpected error occurred ({source}).\r\n{ex.Message}\r\n\r\nThe error has been logged.",
                "Application Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
