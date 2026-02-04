using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.DAL.Repositories
{
    /// <summary>
    /// Provides CRUD operations for medicines and inventory.
    /// </summary>
    public sealed class PharmacyRepository : RepositoryBase
    {
        /// <summary>
        /// Gets all medicines.
        /// </summary>
        public IEnumerable<Medicine> GetAllMedicines()
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM Medicines ORDER BY MedicineName";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Query<Medicine>(sql).ToList();
                }
            }, "GetAllMedicines");
        }

        /// <summary>
        /// Gets all medicines asynchronously.
        /// </summary>
        public Task<List<Medicine>> GetAllMedicinesAsync()
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM Medicines ORDER BY MedicineName";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<Medicine>(sql).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "GetAllMedicinesAsync");
        }

        /// <summary>
        /// Gets a medicine by identifier.
        /// </summary>
        /// <param name="medicineId">Medicine identifier.</param>
        public Medicine GetMedicineById(int medicineId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM Medicines WHERE MedicineID = @MedicineID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.QuerySingleOrDefault<Medicine>(sql, new { MedicineID = medicineId });
                }
            }, "GetMedicineById");
        }

        /// <summary>
        /// Gets a medicine by identifier asynchronously.
        /// </summary>
        /// <param name="medicineId">Medicine identifier.</param>
        public Task<Medicine> GetMedicineByIdAsync(int medicineId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM Medicines WHERE MedicineID = @MedicineID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.QuerySingleOrDefaultAsync<Medicine>(sql, new { MedicineID = medicineId }).ConfigureAwait(false);
                }
            }, "GetMedicineByIdAsync");
        }

        /// <summary>
        /// Adds a new medicine and returns the new identifier.
        /// </summary>
        /// <param name="medicine">Medicine entity.</param>
        public int AddMedicine(Medicine medicine)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"INSERT INTO Medicines
                                    (MedicineCode, MedicineName, GenericName, CategoryID, Manufacturer, UnitOfMeasure,
                                     UnitPrice, SellingPrice, ReorderLevel, IsActive)
                                    VALUES (@MedicineCode, @MedicineName, @GenericName, @CategoryID, @Manufacturer, @UnitOfMeasure,
                                            @UnitPrice, @SellingPrice, @ReorderLevel, @IsActive);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = Db.OpenConnection())
                {
                    return connection.ExecuteScalar<int>(sql, medicine);
                }
            }, "AddMedicine");
        }

        /// <summary>
        /// Adds a new medicine asynchronously and returns the new identifier.
        /// </summary>
        /// <param name="medicine">Medicine entity.</param>
        public Task<int> AddMedicineAsync(Medicine medicine)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"INSERT INTO Medicines
                                    (MedicineCode, MedicineName, GenericName, CategoryID, Manufacturer, UnitOfMeasure,
                                     UnitPrice, SellingPrice, ReorderLevel, IsActive)
                                    VALUES (@MedicineCode, @MedicineName, @GenericName, @CategoryID, @Manufacturer, @UnitOfMeasure,
                                            @UnitPrice, @SellingPrice, @ReorderLevel, @IsActive);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteScalarAsync<int>(sql, medicine).ConfigureAwait(false);
                }
            }, "AddMedicineAsync");
        }

        /// <summary>
        /// Updates an existing medicine.
        /// </summary>
        /// <param name="medicine">Medicine entity.</param>
        public bool UpdateMedicine(Medicine medicine)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"UPDATE Medicines SET
                                    MedicineCode = @MedicineCode,
                                    MedicineName = @MedicineName,
                                    GenericName = @GenericName,
                                    CategoryID = @CategoryID,
                                    Manufacturer = @Manufacturer,
                                    UnitOfMeasure = @UnitOfMeasure,
                                    UnitPrice = @UnitPrice,
                                    SellingPrice = @SellingPrice,
                                    ReorderLevel = @ReorderLevel,
                                    IsActive = @IsActive
                                    WHERE MedicineID = @MedicineID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, medicine) > 0;
                }
            }, "UpdateMedicine");
        }

        /// <summary>
        /// Updates an existing medicine asynchronously.
        /// </summary>
        /// <param name="medicine">Medicine entity.</param>
        public Task<bool> UpdateMedicineAsync(Medicine medicine)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"UPDATE Medicines SET
                                    MedicineCode = @MedicineCode,
                                    MedicineName = @MedicineName,
                                    GenericName = @GenericName,
                                    CategoryID = @CategoryID,
                                    Manufacturer = @Manufacturer,
                                    UnitOfMeasure = @UnitOfMeasure,
                                    UnitPrice = @UnitPrice,
                                    SellingPrice = @SellingPrice,
                                    ReorderLevel = @ReorderLevel,
                                    IsActive = @IsActive
                                    WHERE MedicineID = @MedicineID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, medicine).ConfigureAwait(false) > 0;
                }
            }, "UpdateMedicineAsync");
        }

        /// <summary>
        /// Deletes a medicine by identifier.
        /// </summary>
        /// <param name="medicineId">Medicine identifier.</param>
        public bool DeleteMedicine(int medicineId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "DELETE FROM Medicines WHERE MedicineID = @MedicineID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, new { MedicineID = medicineId }) > 0;
                }
            }, "DeleteMedicine");
        }

        /// <summary>
        /// Deletes a medicine by identifier asynchronously.
        /// </summary>
        /// <param name="medicineId">Medicine identifier.</param>
        public Task<bool> DeleteMedicineAsync(int medicineId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "DELETE FROM Medicines WHERE MedicineID = @MedicineID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, new { MedicineID = medicineId }).ConfigureAwait(false) > 0;
                }
            }, "DeleteMedicineAsync");
        }

        /// <summary>
        /// Searches medicines with filters and pagination.
        /// </summary>
        public Task<PagedResult<Medicine>> SearchMedicinesAsync(
            string medicineCode,
            string medicineName,
            int? categoryId,
            bool? isActive,
            int pageNumber,
            int pageSize)
        {
            return ExecuteSafeAsync(async () =>
            {
                var sql = new StringBuilder("SELECT * FROM Medicines WHERE 1=1 ");
                var countSql = new StringBuilder("SELECT COUNT(*) FROM Medicines WHERE 1=1 ");
                var parameters = new DynamicParameters();

                if (!string.IsNullOrWhiteSpace(medicineCode))
                {
                    sql.Append(" AND MedicineCode LIKE @MedicineCode ");
                    countSql.Append(" AND MedicineCode LIKE @MedicineCode ");
                    parameters.Add("@MedicineCode", $"%{medicineCode}%");
                }

                if (!string.IsNullOrWhiteSpace(medicineName))
                {
                    sql.Append(" AND MedicineName LIKE @MedicineName ");
                    countSql.Append(" AND MedicineName LIKE @MedicineName ");
                    parameters.Add("@MedicineName", $"%{medicineName}%");
                }

                if (categoryId.HasValue)
                {
                    sql.Append(" AND CategoryID = @CategoryID ");
                    countSql.Append(" AND CategoryID = @CategoryID ");
                    parameters.Add("@CategoryID", categoryId.Value);
                }

                if (isActive.HasValue)
                {
                    sql.Append(" AND IsActive = @IsActive ");
                    countSql.Append(" AND IsActive = @IsActive ");
                    parameters.Add("@IsActive", isActive.Value);
                }

                sql.Append(" ORDER BY MedicineName ");
                AddPagination(sql, parameters, pageNumber, pageSize);

                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var totalCount = await connection.ExecuteScalarAsync<int>(countSql.ToString(), parameters).ConfigureAwait(false);
                    var items = await connection.QueryAsync<Medicine>(sql.ToString(), parameters).ConfigureAwait(false);

                    return new PagedResult<Medicine>
                    {
                        Items = items.ToList(),
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    };
                }
            }, "SearchMedicinesAsync");
        }

        /// <summary>
        /// Gets inventory items.
        /// </summary>
        public IEnumerable<InventoryItem> GetInventory()
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM Inventory ORDER BY ExpiryDate";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Query<InventoryItem>(sql).ToList();
                }
            }, "GetInventory");
        }

        /// <summary>
        /// Gets inventory items asynchronously.
        /// </summary>
        public Task<List<InventoryItem>> GetInventoryAsync()
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM Inventory ORDER BY ExpiryDate";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<InventoryItem>(sql).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "GetInventoryAsync");
        }

        /// <summary>
        /// Adds an inventory item and returns the new identifier.
        /// </summary>
        public int AddInventoryItem(InventoryItem item)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"INSERT INTO Inventory
                                    (MedicineID, BatchNumber, ExpiryDate, Quantity, PurchasePrice, SellingPrice, Supplier, PurchaseDate, Location)
                                    VALUES (@MedicineID, @BatchNumber, @ExpiryDate, @Quantity, @PurchasePrice, @SellingPrice, @Supplier, @PurchaseDate, @Location);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = Db.OpenConnection())
                {
                    return connection.ExecuteScalar<int>(sql, item);
                }
            }, "AddInventoryItem");
        }

        /// <summary>
        /// Adds an inventory item asynchronously and returns the new identifier.
        /// </summary>
        public Task<int> AddInventoryItemAsync(InventoryItem item)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"INSERT INTO Inventory
                                    (MedicineID, BatchNumber, ExpiryDate, Quantity, PurchasePrice, SellingPrice, Supplier, PurchaseDate, Location)
                                    VALUES (@MedicineID, @BatchNumber, @ExpiryDate, @Quantity, @PurchasePrice, @SellingPrice, @Supplier, @PurchaseDate, @Location);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteScalarAsync<int>(sql, item).ConfigureAwait(false);
                }
            }, "AddInventoryItemAsync");
        }

        /// <summary>
        /// Updates an inventory item.
        /// </summary>
        public bool UpdateInventoryItem(InventoryItem item)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"UPDATE Inventory SET
                                    MedicineID = @MedicineID,
                                    BatchNumber = @BatchNumber,
                                    ExpiryDate = @ExpiryDate,
                                    Quantity = @Quantity,
                                    PurchasePrice = @PurchasePrice,
                                    SellingPrice = @SellingPrice,
                                    Supplier = @Supplier,
                                    PurchaseDate = @PurchaseDate,
                                    Location = @Location
                                    WHERE InventoryID = @InventoryID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, item) > 0;
                }
            }, "UpdateInventoryItem");
        }

        /// <summary>
        /// Updates an inventory item asynchronously.
        /// </summary>
        public Task<bool> UpdateInventoryItemAsync(InventoryItem item)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"UPDATE Inventory SET
                                    MedicineID = @MedicineID,
                                    BatchNumber = @BatchNumber,
                                    ExpiryDate = @ExpiryDate,
                                    Quantity = @Quantity,
                                    PurchasePrice = @PurchasePrice,
                                    SellingPrice = @SellingPrice,
                                    Supplier = @Supplier,
                                    PurchaseDate = @PurchaseDate,
                                    Location = @Location
                                    WHERE InventoryID = @InventoryID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, item).ConfigureAwait(false) > 0;
                }
            }, "UpdateInventoryItemAsync");
        }

        /// <summary>
        /// Deletes an inventory item.
        /// </summary>
        public bool DeleteInventoryItem(int inventoryId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "DELETE FROM Inventory WHERE InventoryID = @InventoryID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, new { InventoryID = inventoryId }) > 0;
                }
            }, "DeleteInventoryItem");
        }

        /// <summary>
        /// Deletes an inventory item asynchronously.
        /// </summary>
        public Task<bool> DeleteInventoryItemAsync(int inventoryId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "DELETE FROM Inventory WHERE InventoryID = @InventoryID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, new { InventoryID = inventoryId }).ConfigureAwait(false) > 0;
                }
            }, "DeleteInventoryItemAsync");
        }

        /// <summary>
        /// Gets pharmacy sales headers.
        /// </summary>
        public IEnumerable<PharmacySale> GetSales()
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM PharmacySales ORDER BY SaleDate DESC";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Query<PharmacySale>(sql).ToList();
                }
            }, "GetSales");
        }

        /// <summary>
        /// Gets pharmacy sales headers asynchronously.
        /// </summary>
        public Task<List<PharmacySale>> GetSalesAsync()
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM PharmacySales ORDER BY SaleDate DESC";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<PharmacySale>(sql).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "GetSalesAsync");
        }

        /// <summary>
        /// Adds a sale and returns the new identifier.
        /// </summary>
        public int AddSale(PharmacySale sale)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"INSERT INTO PharmacySales
                                    (SaleNumber, PatientID, SaleDate, TotalAmount, Discount, NetAmount, PaymentStatus, SoldBy)
                                    VALUES (@SaleNumber, @PatientID, @SaleDate, @TotalAmount, @Discount, @NetAmount, @PaymentStatus, @SoldBy);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = Db.OpenConnection())
                {
                    return connection.ExecuteScalar<int>(sql, sale);
                }
            }, "AddSale");
        }

        /// <summary>
        /// Adds a sale asynchronously and returns the new identifier.
        /// </summary>
        public Task<int> AddSaleAsync(PharmacySale sale)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"INSERT INTO PharmacySales
                                    (SaleNumber, PatientID, SaleDate, TotalAmount, Discount, NetAmount, PaymentStatus, SoldBy)
                                    VALUES (@SaleNumber, @PatientID, @SaleDate, @TotalAmount, @Discount, @NetAmount, @PaymentStatus, @SoldBy);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteScalarAsync<int>(sql, sale).ConfigureAwait(false);
                }
            }, "AddSaleAsync");
        }

        /// <summary>
        /// Updates a sale.
        /// </summary>
        public bool UpdateSale(PharmacySale sale)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"UPDATE PharmacySales SET
                                    SaleNumber = @SaleNumber,
                                    PatientID = @PatientID,
                                    SaleDate = @SaleDate,
                                    TotalAmount = @TotalAmount,
                                    Discount = @Discount,
                                    NetAmount = @NetAmount,
                                    PaymentStatus = @PaymentStatus,
                                    SoldBy = @SoldBy
                                    WHERE SaleID = @SaleID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, sale) > 0;
                }
            }, "UpdateSale");
        }

        /// <summary>
        /// Updates a sale asynchronously.
        /// </summary>
        public Task<bool> UpdateSaleAsync(PharmacySale sale)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"UPDATE PharmacySales SET
                                    SaleNumber = @SaleNumber,
                                    PatientID = @PatientID,
                                    SaleDate = @SaleDate,
                                    TotalAmount = @TotalAmount,
                                    Discount = @Discount,
                                    NetAmount = @NetAmount,
                                    PaymentStatus = @PaymentStatus,
                                    SoldBy = @SoldBy
                                    WHERE SaleID = @SaleID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, sale).ConfigureAwait(false) > 0;
                }
            }, "UpdateSaleAsync");
        }

        /// <summary>
        /// Deletes a sale by identifier.
        /// </summary>
        public bool DeleteSale(int saleId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "DELETE FROM PharmacySales WHERE SaleID = @SaleID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, new { SaleID = saleId }) > 0;
                }
            }, "DeleteSale");
        }

        /// <summary>
        /// Deletes a sale by identifier asynchronously.
        /// </summary>
        public Task<bool> DeleteSaleAsync(int saleId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "DELETE FROM PharmacySales WHERE SaleID = @SaleID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, new { SaleID = saleId }).ConfigureAwait(false) > 0;
                }
            }, "DeleteSaleAsync");
        }

        /// <summary>
        /// Gets sale details for a sale.
        /// </summary>
        public IEnumerable<PharmacySaleDetail> GetSaleDetails(int saleId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "SELECT * FROM PharmacySaleDetails WHERE SaleID = @SaleID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Query<PharmacySaleDetail>(sql, new { SaleID = saleId }).ToList();
                }
            }, "GetSaleDetails");
        }

        /// <summary>
        /// Gets sale details asynchronously.
        /// </summary>
        public Task<List<PharmacySaleDetail>> GetSaleDetailsAsync(int saleId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "SELECT * FROM PharmacySaleDetails WHERE SaleID = @SaleID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<PharmacySaleDetail>(sql, new { SaleID = saleId }).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "GetSaleDetailsAsync");
        }

        /// <summary>
        /// Adds a sale detail and returns the new identifier.
        /// </summary>
        public int AddSaleDetail(PharmacySaleDetail detail)
        {
            return ExecuteSafe(() =>
            {
                const string sql = @"INSERT INTO PharmacySaleDetails
                                    (SaleID, MedicineID, BatchNumber, Quantity, UnitPrice)
                                    VALUES (@SaleID, @MedicineID, @BatchNumber, @Quantity, @UnitPrice);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = Db.OpenConnection())
                {
                    return connection.ExecuteScalar<int>(sql, detail);
                }
            }, "AddSaleDetail");
        }

        /// <summary>
        /// Adds a sale detail asynchronously and returns the new identifier.
        /// </summary>
        public Task<int> AddSaleDetailAsync(PharmacySaleDetail detail)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = @"INSERT INTO PharmacySaleDetails
                                    (SaleID, MedicineID, BatchNumber, Quantity, UnitPrice)
                                    VALUES (@SaleID, @MedicineID, @BatchNumber, @Quantity, @UnitPrice);
                                    SELECT LAST_INSERT_ID();";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteScalarAsync<int>(sql, detail).ConfigureAwait(false);
                }
            }, "AddSaleDetailAsync");
        }

        /// <summary>
        /// Deletes a sale detail.
        /// </summary>
        public bool DeleteSaleDetail(int saleDetailId)
        {
            return ExecuteSafe(() =>
            {
                const string sql = "DELETE FROM PharmacySaleDetails WHERE SaleDetailID = @SaleDetailID";
                using (var connection = Db.OpenConnection())
                {
                    return connection.Execute(sql, new { SaleDetailID = saleDetailId }) > 0;
                }
            }, "DeleteSaleDetail");
        }

        /// <summary>
        /// Deletes a sale detail asynchronously.
        /// </summary>
        public Task<bool> DeleteSaleDetailAsync(int saleDetailId)
        {
            return ExecuteSafeAsync(async () =>
            {
                const string sql = "DELETE FROM PharmacySaleDetails WHERE SaleDetailID = @SaleDetailID";
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    return await connection.ExecuteAsync(sql, new { SaleDetailID = saleDetailId }).ConfigureAwait(false) > 0;
                }
            }, "DeleteSaleDetailAsync");
        }

        /// <summary>
        /// Executes a stored procedure and returns typed results.
        /// </summary>
        public IEnumerable<T> ExecuteStoredProcedure<T>(string procedureName, object parameters = null)
        {
            return ExecuteSafe(() =>
            {
                using (var connection = Db.OpenConnection())
                {
                    return connection.Query<T>(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure).ToList();
                }
            }, "ExecuteStoredProcedure");
        }

        /// <summary>
        /// Executes a stored procedure asynchronously and returns typed results.
        /// </summary>
        public Task<List<T>> ExecuteStoredProcedureAsync<T>(string procedureName, object parameters = null)
        {
            return ExecuteSafeAsync(async () =>
            {
                using (var connection = await Db.OpenConnectionAsync().ConfigureAwait(false))
                {
                    var results = await connection.QueryAsync<T>(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure).ConfigureAwait(false);
                    return results.ToList();
                }
            }, "ExecuteStoredProcedureAsync");
        }
    }
}
