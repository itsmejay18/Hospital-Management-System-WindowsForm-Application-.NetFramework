using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using HospitalManagementSystem.BLL.Services;
using HospitalManagementSystem.Helpers;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.UserControls
{
    public partial class ucBilling : UserControl
    {
        private readonly BindingList<Invoice> _invoices = new BindingList<Invoice>();
        private readonly BillingService _service = new BillingService();
        private readonly List<Invoice> _allInvoices = new List<Invoice>();

        public ucBilling()
        {
            InitializeComponent();
            ConfigureGrid();
            HookEvents();
            ApplyTheme();
            Load += ucBilling_Load;
        }

        private void ConfigureGrid()
        {
            dgvBilling.AutoGenerateColumns = false;
            dgvBilling.DataSource = _invoices;
            dgvBilling.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            colInvoice.FillWeight = 22F;
            colPatient.FillWeight = 36F;
            colAmount.FillWeight = 20F;
            colStatus.FillWeight = 22F;
            colAmount.DefaultCellStyle.Format = "N2";
        }

        private void HookEvents()
        {
            btnSearch.Click += btnSearch_Click;
            btnRefresh.Click += btnRefresh_Click;
            txtSearch.KeyDown += txtSearch_KeyDown;
            btnProcessPayment.Click += btnProcessPayment_Click;
            dgvBilling.SelectionChanged += dgvBilling_SelectionChanged;
        }

        private async void ucBilling_Load(object sender, EventArgs e)
        {
            await ReloadAsync().ConfigureAwait(true);
        }

        private async System.Threading.Tasks.Task ReloadAsync(int? selectInvoiceId = null)
        {
            try
            {
                UseWaitCursor = true;
                var list = await _service.GetInvoicesAsync().ConfigureAwait(true);
                _allInvoices.Clear();
                _allInvoices.AddRange(list);
                ApplyFilter(txtSearch.Text, selectInvoiceId);
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

        private void ApplyFilter(string searchTerm, int? preferredInvoiceId = null)
        {
            var term = (searchTerm ?? string.Empty).Trim();
            var filtered = string.IsNullOrWhiteSpace(term)
                ? _allInvoices
                : _allInvoices.Where(x =>
                    ContainsInsensitive(x.InvoiceNumber, term)
                    || ContainsInsensitive(x.PatientName, term)
                    || ContainsInsensitive(x.Status, term)).ToList();

            _invoices.RaiseListChangedEvents = false;
            _invoices.Clear();
            foreach (var item in filtered)
            {
                _invoices.Add(item);
            }

            _invoices.RaiseListChangedEvents = true;
            _invoices.ResetBindings();

            RestoreSelection(preferredInvoiceId);
        }

        private static bool ContainsInsensitive(string value, string searchText)
        {
            return !string.IsNullOrWhiteSpace(value)
                   && value.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void RestoreSelection(int? invoiceId)
        {
            if (dgvBilling.Rows.Count == 0)
            {
                PopulateDetails(null);
                btnProcessPayment.Enabled = false;
                return;
            }

            DataGridViewRow targetRow = null;
            if (invoiceId.HasValue)
            {
                foreach (DataGridViewRow row in dgvBilling.Rows)
                {
                    if (row.DataBoundItem is Invoice item && item.InvoiceID == invoiceId.Value)
                    {
                        targetRow = row;
                        break;
                    }
                }
            }

            if (targetRow == null)
            {
                targetRow = dgvBilling.Rows[0];
            }

            if (targetRow.Cells.Count > 0)
            {
                dgvBilling.CurrentCell = targetRow.Cells[0];
            }

            PopulateDetails(targetRow.DataBoundItem as Invoice);
            btnProcessPayment.Enabled = true;
        }

        private Invoice GetSelectedInvoice()
        {
            return dgvBilling.CurrentRow?.DataBoundItem as Invoice;
        }

        private void PopulateDetails(Invoice invoice)
        {
            if (invoice == null)
            {
                txtInvoiceNo.Clear();
                txtPatient.Clear();
                txtStatus.Clear();
                txtTotalAmount.Clear();
                txtDiscount.Clear();
                txtTax.Clear();
                txtGrandTotal.Clear();
                dtpInvoiceDate.Value = DateTime.Today;
                dtpDueDate.Value = DateTime.Today;
                txtNotes.Clear();
                return;
            }

            txtInvoiceNo.Text = invoice.InvoiceNumber;
            txtPatient.Text = invoice.PatientName;
            txtStatus.Text = invoice.Status;
            txtTotalAmount.Text = invoice.TotalAmount.ToString("N2");
            txtDiscount.Text = invoice.Discount.ToString("N2");
            txtTax.Text = invoice.TaxAmount.ToString("N2");
            txtGrandTotal.Text = invoice.GrandTotal.ToString("N2");
            dtpInvoiceDate.Value = invoice.InvoiceDate ?? DateTime.Today;
            dtpDueDate.Value = invoice.DueDate ?? DateTime.Today;
            txtNotes.Text = invoice.Notes;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            ApplyFilter(txtSearch.Text);
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            await ReloadAsync().ConfigureAwait(true);
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.SuppressKeyPress = true;
            ApplyFilter(txtSearch.Text);
        }

        private async void btnProcessPayment_Click(object sender, EventArgs e)
        {
            var invoice = GetSelectedInvoice();
            if (invoice == null)
            {
                MessageBox.Show("Select an invoice first.", "Billing", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dlg = new Forms.Shared.frmInvoicePayment(invoice.InvoiceID))
            {
                IWin32Window owner = FindForm();
                if (owner == null)
                {
                    owner = this;
                }
                if (dlg.ShowDialog(owner) == DialogResult.OK)
                {
                    await ReloadAsync(invoice.InvoiceID).ConfigureAwait(true);
                }
            }
        }

        private void dgvBilling_SelectionChanged(object sender, EventArgs e)
        {
            PopulateDetails(GetSelectedInvoice());
            btnProcessPayment.Enabled = GetSelectedInvoice() != null;
        }

        private void ApplyTheme()
        {
            ThemeManager.ApplyControlTheme(this);
            ThemeManager.StyleMasterDetailModule(this, pnlSearch, pnlButtons, splitMain, grpDetails);
            ThemeManager.StyleButton(btnSearch, ThemeButtonKind.Primary);
            ThemeManager.StyleButton(btnRefresh, ThemeButtonKind.Secondary);
            ThemeManager.StyleButton(btnProcessPayment, ThemeButtonKind.Primary);
            ThemeManager.StyleSearchTextBox(txtSearch, "Search invoice # / patient / status");

            txtInvoiceNo.ReadOnly = true;
            txtPatient.ReadOnly = true;
            txtStatus.ReadOnly = true;
            txtTotalAmount.ReadOnly = true;
            txtDiscount.ReadOnly = true;
            txtTax.ReadOnly = true;
            txtGrandTotal.ReadOnly = true;
            txtNotes.ReadOnly = true;

            txtInvoiceNo.BackColor = ThemeManager.Colors.SurfaceMuted;
            txtPatient.BackColor = ThemeManager.Colors.SurfaceMuted;
            txtStatus.BackColor = ThemeManager.Colors.SurfaceMuted;
            txtTotalAmount.BackColor = ThemeManager.Colors.SurfaceMuted;
            txtDiscount.BackColor = ThemeManager.Colors.SurfaceMuted;
            txtTax.BackColor = ThemeManager.Colors.SurfaceMuted;
            txtGrandTotal.BackColor = ThemeManager.Colors.SurfaceMuted;
            txtNotes.BackColor = ThemeManager.Colors.SurfaceMuted;

            dtpInvoiceDate.Enabled = false;
            dtpDueDate.Enabled = false;
        }
    }
}
