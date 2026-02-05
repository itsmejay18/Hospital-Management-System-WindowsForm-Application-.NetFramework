using System;
using System.Windows.Forms;
using HospitalManagementSystem.DAL.Repositories;
using HospitalManagementSystem.Helpers;

namespace HospitalManagementSystem.Forms.Shared
{
    public partial class frmInvoicePayment : Form
    {
        private readonly BillingRepository _billing = new BillingRepository();
        private readonly int _invoiceId;

        public frmInvoicePayment(int invoiceId)
        {
            InitializeComponent();
            _invoiceId = invoiceId;
            cboMethod.SelectedIndex = 0;
        }

        private async void btnProcess_Click(object sender, EventArgs e)
        {
            try
            {
                var amount = numAmount.Value;
                if (amount <= 0)
                {
                    MessageBox.Show("Enter a valid amount.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = await _billing.ProcessPaymentAsync(
                    _invoiceId,
                    cboMethod.Text,
                    amount,
                    txtReference.Text.Trim(),
                    UserSession.CurrentUser?.UserID ?? 1).ConfigureAwait(true);

                MessageBox.Show($"Payment saved. Invoice status: {result.InvoiceStatus}", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Payment failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
