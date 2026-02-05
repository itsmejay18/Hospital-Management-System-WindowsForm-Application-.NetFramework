using System;
using System.Windows.Forms;

namespace HospitalManagementSystem.UserControls
{
    public partial class ucRoleDashboard : UserControl
    {
        public ucRoleDashboard()
        {
            InitializeComponent();
        }

        public void Configure(string roleName, string username)
        {
            var normalizedRole = string.IsNullOrWhiteSpace(roleName) ? "User" : roleName.Trim();
            lblRoleTitle.Text = $"{normalizedRole} Dashboard";
            lblWelcome.Text = $"Welcome, {username}";
            lblHint.Text = BuildHint(normalizedRole);
        }

        private static string BuildHint(string roleName)
        {
            if (string.Equals(roleName, "Doctor", StringComparison.OrdinalIgnoreCase))
            {
                return "Use Appointments for your queue and Patients for medical records.";
            }

            if (string.Equals(roleName, "Nurse", StringComparison.OrdinalIgnoreCase))
            {
                return "Use Patients and Appointments to manage daily care workflow.";
            }

            if (string.Equals(roleName, "Receptionist", StringComparison.OrdinalIgnoreCase))
            {
                return "Use Patients, Doctors, Appointments, and Billing for front desk operations.";
            }

            return "Use the left menu to access your allowed modules.";
        }
    }
}
