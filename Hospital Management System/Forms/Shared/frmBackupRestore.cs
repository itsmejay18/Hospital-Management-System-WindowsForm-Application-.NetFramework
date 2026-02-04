using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace HospitalManagementSystem.Forms.Shared
{
    public partial class frmBackupRestore : Form
    {
        public frmBackupRestore()
        {
            InitializeComponent();
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            try
            {
                using (var sfd = new SaveFileDialog { Filter = "SQL (*.sql)|*.sql" })
                {
                    if (sfd.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }

                    var args = $"--user=root --password=root HospitalManagementSystem -r \"{sfd.FileName}\"";
                    RunProcess("mysqldump", args);
                    MessageBox.Show("Backup completed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Backup failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            try
            {
                using (var ofd = new OpenFileDialog { Filter = "SQL (*.sql)|*.sql" })
                {
                    if (ofd.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }

                    var args = $"--user=root --password=root HospitalManagementSystem < \"{ofd.FileName}\"";
                    RunProcess("mysql", args, true);
                    MessageBox.Show("Restore completed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Restore failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void RunProcess(string fileName, string args, bool useShell = false)
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = args,
                CreateNoWindow = true,
                UseShellExecute = useShell,
                RedirectStandardOutput = !useShell,
                RedirectStandardError = !useShell
            };

            using (var process = Process.Start(psi))
            {
                if (process == null)
                {
                    throw new InvalidOperationException("Unable to start process.");
                }

                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    var err = process.StandardError.ReadToEnd();
                    throw new InvalidOperationException(err);
                }
            }
        }
    }
}
