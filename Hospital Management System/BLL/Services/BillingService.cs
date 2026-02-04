using System.Collections.Generic;
using System.Threading.Tasks;
using HospitalManagementSystem.DAL.Repositories;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.BLL.Services
{
    /// <summary>
    /// Provides billing business logic.
    /// </summary>
    public sealed class BillingService
    {
        private readonly BillingRepository _repository = new BillingRepository();

        /// <summary>
        /// Gets all invoices.
        /// </summary>
        public Task<List<Invoice>> GetInvoicesAsync()
        {
            return _repository.GetAllInvoicesAsync();
        }
    }
}
