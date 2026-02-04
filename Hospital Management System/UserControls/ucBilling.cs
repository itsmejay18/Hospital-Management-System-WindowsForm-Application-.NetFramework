using System;
using System.ComponentModel;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.UserControls
{
    public partial class ucBilling : UserControl
    {
        private readonly BindingList<Invoice> _invoices = new BindingList<Invoice>();
        private readonly BillingService _service = new BillingService();

        public ucBilling()
        {
            InitializeComponent();
            dgvBilling.AutoGenerateColumns = false;
            dgvBilling.DataSource = _invoices;
            Load += ucBilling_Load;
            btnProcessPayment.Click += btnProcessPayment_Click;
        }

        private async void ucBilling_Load(object sender, EventArgs e)
        {
            await ucBilling_LoadAsync().ConfigureAwait(false);
        }

        private void btnProcessPayment_Click(object sender, EventArgs e)
        {
            if (dgvBilling.CurrentRow?.DataBoundItem is Invoice invoice)
            {
                using (var dlg = new Forms.Shared.frmInvoicePayment(invoice.InvoiceID))
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        _ = ucBilling_LoadAsync();
                    }
                }
            }
        }

        private async System.Threading.Tasks.Task ucBilling_LoadAsync()
        {
            try
            {
                UseWaitCursor = true;
                _invoices.Clear();
                var list = await _service.GetInvoicesAsync().ConfigureAwait(true);
                foreach (var item in list)
                {
                    _invoices.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load invoices: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }
    }
}
