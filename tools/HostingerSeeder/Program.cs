using MySqlConnector;

const string DefaultConnectionString =
    "server=srv1237.hstgr.io;port=3306;database=u621755393_hospitalmanage;user id=u621755393_hospitalmanage;password=Dssc@2026;pooling=True;charset=utf8mb4;AllowPublicKeyRetrieval=True";

var connectionString = Environment.GetEnvironmentVariable("HMS_CONNECTION_STRING");
if (string.IsNullOrWhiteSpace(connectionString))
{
    connectionString = DefaultConnectionString;
}

var seeder = new HostingerSeeder(connectionString);
await seeder.RunAsync();

internal sealed class HostingerSeeder
{
    private const int TargetCount = 30;
    private readonly string _connectionString;
    private MySqlConnection _connection = null!;
    private MySqlTransaction? _transaction;
    private readonly Dictionary<string, int> _roleIds = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _specializationIds = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _serviceIds = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _medicineIds = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _labTestIds = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _wardIds = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _roomIds = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _userIds = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _doctorIds = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _patientIds = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _appointmentIds = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _visitIds = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _invoiceIds = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _saleIds = new(StringComparer.OrdinalIgnoreCase);
    private string _seedPasswordHash = string.Empty;

    public HostingerSeeder(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task RunAsync()
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync().ConfigureAwait(false);
        await using var transaction = await connection.BeginTransactionAsync().ConfigureAwait(false);

        _connection = connection;
        _transaction = transaction;

        try
        {
            _seedPasswordHash = await GetSeedPasswordHashAsync().ConfigureAwait(false);

            await SeedRolesAsync().ConfigureAwait(false);
            await LoadRoleIdsAsync().ConfigureAwait(false);
            await SeedSpecializationsAsync().ConfigureAwait(false);
            await SeedServiceCatalogAsync().ConfigureAwait(false);
            await SeedMedicineCatalogAsync().ConfigureAwait(false);
            await SeedLabTestsAsync().ConfigureAwait(false);
            await SeedWardsAndRoomsAsync().ConfigureAwait(false);
            await SeedSystemSettingsAsync().ConfigureAwait(false);

            await SeedSupportUsersAsync().ConfigureAwait(false);
            await SeedDoctorsAsync().ConfigureAwait(false);
            await SeedPatientsAsync().ConfigureAwait(false);
            await SeedAppointmentsAsync().ConfigureAwait(false);
            await SeedVisitsAsync().ConfigureAwait(false);
            await SeedPrescriptionsAsync().ConfigureAwait(false);
            await SeedAdmissionsAsync().ConfigureAwait(false);
            await SeedInvoicesAsync().ConfigureAwait(false);
            await SeedLabOrdersAsync().ConfigureAwait(false);
            await SeedPharmacySalesAsync().ConfigureAwait(false);
            await SeedNotificationsAsync().ConfigureAwait(false);
            await SeedAuditLogsAsync().ConfigureAwait(false);
            await RefreshBedAvailabilityAsync().ConfigureAwait(false);

            await transaction.CommitAsync().ConfigureAwait(false);
            _transaction = null;
            Console.WriteLine("Seed completed successfully.");
            await PrintCountsAsync().ConfigureAwait(false);
        }
        catch
        {
            if (_transaction is not null)
            {
                await transaction.RollbackAsync().ConfigureAwait(false);
            }
            throw;
        }
    }

    private async Task<string> GetSeedPasswordHashAsync()
    {
        var hash = await ScalarAsync<string>(
            "SELECT PasswordHash FROM users WHERE Username = 'admin' LIMIT 1;").ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new InvalidOperationException("Existing admin password hash was not found.");
        }

        return hash;
    }

    private async Task SeedRolesAsync()
    {
        var officialRoles = new[]
        {
            ("Administrator", "Full system access"),
            ("SuperAdmin", "Installation super administrator"),
            ("Doctor", "Medical staff"),
            ("Nurse", "Nursing staff"),
            ("Receptionist", "Front desk"),
            ("Pharmacist", "Pharmacy management"),
            ("Lab Technician", "Laboratory test management"),
            ("Accountant", "Billing and finance"),
            ("HR Manager", "Human resources")
        };

        foreach (var role in officialRoles)
        {
            if (!await ExistsAsync("SELECT 1 FROM userroles WHERE RoleName = @name LIMIT 1;", ("@name", role.Item1)).ConfigureAwait(false))
            {
                await ExecuteAsync(
                    "INSERT INTO userroles (RoleName, Description) VALUES (@name, @description);",
                    ("@name", role.Item1),
                    ("@description", role.Item2)).ConfigureAwait(false);
            }
        }

        for (var i = 1; i <= 21; i++)
        {
            var roleName = $"Seed Role {i:00}";
            if (!await ExistsAsync("SELECT 1 FROM userroles WHERE RoleName = @name LIMIT 1;", ("@name", roleName)).ConfigureAwait(false))
            {
                await ExecuteAsync(
                    "INSERT INTO userroles (RoleName, Description) VALUES (@name, @description);",
                    ("@name", roleName),
                    ("@description", $"Generated sample role {i:00}.")).ConfigureAwait(false);
            }
        }
    }

    private async Task LoadRoleIdsAsync()
    {
        _roleIds.Clear();
        await using var command = CreateCommand("SELECT RoleName, RoleID FROM userroles;");
        await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            _roleIds[reader.GetString("RoleName")] = reader.GetInt32("RoleID");
        }
    }

    private async Task SeedSpecializationsAsync()
    {
        var items = new[]
        {
            new SpecializationSeed("SP01", "Cardiology", "Heart and vascular care", "Cardiology"),
            new SpecializationSeed("SP02", "Pediatrics", "Child health and wellness", "Pediatrics"),
            new SpecializationSeed("SP03", "General Medicine", "Primary adult care", "Internal Medicine"),
            new SpecializationSeed("SP04", "General Surgery", "Operative and perioperative care", "Surgery"),
            new SpecializationSeed("SP05", "Gynecology", "Women's health services", "OB-GYN"),
            new SpecializationSeed("SP06", "Neurology", "Neurologic conditions", "Neurology"),
            new SpecializationSeed("SP07", "Orthopedics", "Bones and joints", "Orthopedics"),
            new SpecializationSeed("SP08", "ENT", "Ear, nose, and throat care", "ENT"),
            new SpecializationSeed("SP09", "Dermatology", "Skin and hair conditions", "Dermatology"),
            new SpecializationSeed("SP10", "Psychiatry", "Mental health care", "Psychiatry"),
            new SpecializationSeed("SP11", "Pulmonology", "Respiratory disease care", "Pulmonology"),
            new SpecializationSeed("SP12", "Nephrology", "Kidney health care", "Nephrology"),
            new SpecializationSeed("SP13", "Endocrinology", "Hormonal and metabolic care", "Endocrinology"),
            new SpecializationSeed("SP14", "Gastroenterology", "Digestive system care", "Gastroenterology"),
            new SpecializationSeed("SP15", "Oncology", "Cancer care and follow-up", "Oncology"),
            new SpecializationSeed("SP16", "Ophthalmology", "Eye care and surgery", "Ophthalmology"),
            new SpecializationSeed("SP17", "Urology", "Urinary tract care", "Urology"),
            new SpecializationSeed("SP18", "Radiology", "Imaging interpretation", "Radiology"),
            new SpecializationSeed("SP19", "Anesthesiology", "Perioperative pain management", "Anesthesiology"),
            new SpecializationSeed("SP20", "Family Medicine", "Continuity outpatient care", "Family Medicine"),
            new SpecializationSeed("SP21", "Infectious Disease", "Complex infection management", "Infectious Disease"),
            new SpecializationSeed("SP22", "Rheumatology", "Autoimmune and joint disorders", "Rheumatology"),
            new SpecializationSeed("SP23", "Hematology", "Blood disorder management", "Hematology"),
            new SpecializationSeed("SP24", "Geriatrics", "Senior care coordination", "Geriatrics"),
            new SpecializationSeed("SP25", "Emergency Medicine", "Emergency department care", "Emergency"),
            new SpecializationSeed("SP26", "Rehabilitation Medicine", "Physical recovery programs", "Rehabilitation"),
            new SpecializationSeed("SP27", "Pathology", "Diagnostic laboratory pathology", "Pathology"),
            new SpecializationSeed("SP28", "Obstetrics", "Pregnancy and delivery care", "OB-GYN"),
            new SpecializationSeed("SP29", "Allergy and Immunology", "Allergy and immune care", "Allergy"),
            new SpecializationSeed("SP30", "Pain Management", "Chronic pain treatment", "Pain Management")
        };

        foreach (var item in items)
        {
            if (!await ExistsAsync("SELECT 1 FROM specializations WHERE SpecializationCode = @code LIMIT 1;", ("@code", item.Code)).ConfigureAwait(false))
            {
                await ExecuteAsync(
                    @"INSERT INTO specializations (SpecializationCode, SpecializationName, Description, Department)
                      VALUES (@code, @name, @description, @department);",
                    ("@code", item.Code),
                    ("@name", item.Name),
                    ("@description", item.Description),
                    ("@department", item.Department)).ConfigureAwait(false);
            }
        }

        _specializationIds.Clear();
        await using var command = CreateCommand("SELECT SpecializationCode, SpecializationID FROM specializations;");
        await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            _specializationIds[reader.GetString("SpecializationCode")] = reader.GetInt32("SpecializationID");
        }
    }

    private async Task SeedServiceCatalogAsync()
    {
        for (var i = 1; i <= TargetCount; i++)
        {
            var categoryName = $"Seed Service Category {i:00}";
            if (!await ExistsAsync("SELECT 1 FROM servicecategories WHERE CategoryName = @name LIMIT 1;", ("@name", categoryName)).ConfigureAwait(false))
            {
                await ExecuteAsync(
                    "INSERT INTO servicecategories (CategoryName, Description) VALUES (@name, @description);",
                    ("@name", categoryName),
                    ("@description", $"Sample category {i:00} for seeded billing items.")).ConfigureAwait(false);
            }
        }

        for (var i = 1; i <= TargetCount; i++)
        {
            var code = $"SRVSD{i:000}";
            if (!await ExistsAsync("SELECT 1 FROM services WHERE ServiceCode = @code LIMIT 1;", ("@code", code)).ConfigureAwait(false))
            {
                var categoryId = await ScalarAsync<int>(
                    "SELECT CategoryID FROM servicecategories WHERE CategoryName = @name LIMIT 1;",
                    ("@name", $"Seed Service Category {i:00}")).ConfigureAwait(false);
                await ExecuteAsync(
                    @"INSERT INTO services (ServiceCode, ServiceName, CategoryID, Price, TaxRate, IsActive)
                      VALUES (@code, @name, @categoryId, @price, @taxRate, 1);",
                    ("@code", code),
                    ("@name", $"Seed Clinical Service {i:00}"),
                    ("@categoryId", categoryId),
                    ("@price", 850m + (i * 55m)),
                    ("@taxRate", 12m)).ConfigureAwait(false);
            }
        }

        _serviceIds.Clear();
        await using var command = CreateCommand("SELECT ServiceCode, ServiceID FROM services;");
        await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            _serviceIds[reader.GetString("ServiceCode")] = reader.GetInt32("ServiceID");
        }
    }

    private async Task SeedMedicineCatalogAsync()
    {
        for (var i = 1; i <= TargetCount; i++)
        {
            var categoryName = $"Seed Medicine Category {i:00}";
            if (!await ExistsAsync("SELECT 1 FROM medicinecategories WHERE CategoryName = @name LIMIT 1;", ("@name", categoryName)).ConfigureAwait(false))
            {
                await ExecuteAsync(
                    "INSERT INTO medicinecategories (CategoryName, Description) VALUES (@name, @description);",
                    ("@name", categoryName),
                    ("@description", $"Sample medicine category {i:00}.")).ConfigureAwait(false);
            }
        }

        for (var i = 1; i <= TargetCount; i++)
        {
            var code = $"MEDSD{i:000}";
            if (!await ExistsAsync("SELECT 1 FROM medicines WHERE MedicineCode = @code LIMIT 1;", ("@code", code)).ConfigureAwait(false))
            {
                var categoryId = await ScalarAsync<int>(
                    "SELECT CategoryID FROM medicinecategories WHERE CategoryName = @name LIMIT 1;",
                    ("@name", $"Seed Medicine Category {i:00}")).ConfigureAwait(false);
                await ExecuteAsync(
                    @"INSERT INTO medicines
                      (MedicineCode, MedicineName, GenericName, CategoryID, Manufacturer, UnitOfMeasure, UnitPrice, SellingPrice, ReorderLevel, IsActive)
                      VALUES (@code, @name, @generic, @categoryId, @manufacturer, @uom, @unitPrice, @sellingPrice, @reorderLevel, 1);",
                    ("@code", code),
                    ("@name", $"Seed Medicine {i:00}"),
                    ("@generic", $"Generic Compound {i:00}"),
                    ("@categoryId", categoryId),
                    ("@manufacturer", $"Seed Pharma {1 + ((i - 1) % 6)}"),
                    ("@uom", i % 3 == 0 ? "Vial" : i % 2 == 0 ? "Capsule" : "Tablet"),
                    ("@unitPrice", 18m + (i * 2m)),
                    ("@sellingPrice", 30m + (i * 3m)),
                    ("@reorderLevel", 10 + i)).ConfigureAwait(false);
            }
        }

        _medicineIds.Clear();
        await using (var command = CreateCommand("SELECT MedicineCode, MedicineID FROM medicines;"))
        await using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
        {
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                _medicineIds[reader.GetString("MedicineCode")] = reader.GetInt32("MedicineID");
            }
        }

        for (var i = 1; i <= TargetCount; i++)
        {
            var medicineCode = $"MEDSD{i:000}";
            var medicineId = _medicineIds[medicineCode];
            if (!await ExistsAsync("SELECT 1 FROM inventory WHERE MedicineID = @medicineId LIMIT 1;", ("@medicineId", medicineId)).ConfigureAwait(false))
            {
                await ExecuteAsync(
                    @"INSERT INTO inventory
                      (MedicineID, BatchNumber, ExpiryDate, Quantity, PurchasePrice, SellingPrice, Supplier, PurchaseDate, Location)
                      VALUES (@medicineId, @batch, @expiryDate, @quantity, @purchasePrice, @sellingPrice, @supplier, @purchaseDate, @location);",
                    ("@medicineId", medicineId),
                    ("@batch", $"BATCH-SD-{i:000}"),
                    ("@expiryDate", DateTime.Today.AddDays(180 + (i * 12))),
                    ("@quantity", 70 + (i * 3)),
                    ("@purchasePrice", 15m + (i * 1.5m)),
                    ("@sellingPrice", 30m + (i * 3m)),
                    ("@supplier", $"Supplier {1 + ((i - 1) % 8)}"),
                    ("@purchaseDate", DateTime.Today.AddDays(-(45 + i))),
                    ("@location", $"Rack-{1 + ((i - 1) % 10):00}")).ConfigureAwait(false);
            }
        }
    }

    private async Task SeedLabTestsAsync()
    {
        for (var i = 1; i <= TargetCount; i++)
        {
            var code = $"LABTSD{i:000}";
            if (!await ExistsAsync("SELECT 1 FROM labtests WHERE TestCode = @code LIMIT 1;", ("@code", code)).ConfigureAwait(false))
            {
                await ExecuteAsync(
                    @"INSERT INTO labtests (TestCode, TestName, Category, NormalRange, Unit, Price)
                      VALUES (@code, @name, @category, @normalRange, @unit, @price);",
                    ("@code", code),
                    ("@name", $"Seed Laboratory Test {i:00}"),
                    ("@category", i % 3 == 0 ? "Chemistry" : i % 2 == 0 ? "Hematology" : "Immunology"),
                    ("@normalRange", i % 2 == 0 ? "70-140" : "4.0-10.0"),
                    ("@unit", i % 2 == 0 ? "mg/dL" : "x10^9/L"),
                    ("@price", 400m + (i * 45m))).ConfigureAwait(false);
            }
        }

        _labTestIds.Clear();
        await using var command = CreateCommand("SELECT TestCode, TestID FROM labtests;");
        await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            _labTestIds[reader.GetString("TestCode")] = reader.GetInt32("TestID");
        }
    }

    private async Task SeedWardsAndRoomsAsync()
    {
        for (var i = 1; i <= TargetCount; i++)
        {
            var wardCode = $"WRDSD{i:000}";
            if (!await ExistsAsync("SELECT 1 FROM wards WHERE WardCode = @code LIMIT 1;", ("@code", wardCode)).ConfigureAwait(false))
            {
                await ExecuteAsync(
                    @"INSERT INTO wards
                      (WardCode, WardName, WardType, Description, TotalBeds, AvailableBeds, ChargePerDay, IsActive)
                      VALUES (@code, @name, @type, @description, @totalBeds, @availableBeds, @chargePerDay, 1);",
                    ("@code", wardCode),
                    ("@name", $"Seed Ward {i:00}"),
                    ("@type", i % 4 == 0 ? "ICU" : i % 3 == 0 ? "Private" : i % 2 == 0 ? "Semi-Private" : "General"),
                    ("@description", $"Seeded ward {i:00} for admissions and room occupancy."),
                    ("@totalBeds", 2),
                    ("@availableBeds", 2),
                    ("@chargePerDay", 1400m + (i * 95m))).ConfigureAwait(false);
            }
        }

        _wardIds.Clear();
        await using (var command = CreateCommand("SELECT WardCode, WardID FROM wards;"))
        await using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
        {
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                _wardIds[reader.GetString("WardCode")] = reader.GetInt32("WardID");
            }
        }

        for (var i = 1; i <= TargetCount; i++)
        {
            var roomNumber = $"RMSD{i:000}";
            if (!await ExistsAsync("SELECT 1 FROM rooms WHERE RoomNumber = @number LIMIT 1;", ("@number", roomNumber)).ConfigureAwait(false))
            {
                await ExecuteAsync(
                    @"INSERT INTO rooms
                      (RoomNumber, WardID, RoomType, TotalBeds, AvailableBeds, Facilities, RatePerDay, Status)
                      VALUES (@number, @wardId, @roomType, 2, 2, @facilities, @ratePerDay, 'Available');",
                    ("@number", roomNumber),
                    ("@wardId", _wardIds[$"WRDSD{i:000}"]),
                    ("@roomType", i % 4 == 0 ? "ICU" : i % 3 == 0 ? "Private" : "Standard"),
                    ("@facilities", "Air conditioning, oxygen port, bedside monitor"),
                    ("@ratePerDay", 1800m + (i * 110m))).ConfigureAwait(false);
            }
        }

        _roomIds.Clear();
        await using var roomCommand = CreateCommand("SELECT RoomNumber, RoomID FROM rooms;");
        await using var roomReader = await roomCommand.ExecuteReaderAsync().ConfigureAwait(false);
        while (await roomReader.ReadAsync().ConfigureAwait(false))
        {
            _roomIds[roomReader.GetString("RoomNumber")] = roomReader.GetInt32("RoomID");
        }
    }

    private async Task SeedSystemSettingsAsync()
    {
        for (var i = 1; i <= TargetCount; i++)
        {
            var key = $"Seed.Setting.{i:00}";
            if (!await ExistsAsync("SELECT 1 FROM systemsettings WHERE SettingKey = @key LIMIT 1;", ("@key", key)).ConfigureAwait(false))
            {
                await ExecuteAsync(
                    @"INSERT INTO systemsettings (SettingKey, SettingValue, Description, Category)
                      VALUES (@key, @value, @description, @category);",
                    ("@key", key),
                    ("@value", $"Value-{i:00}"),
                    ("@description", $"Seed system setting {i:00}."),
                    ("@category", i % 2 == 0 ? "General" : "Billing")).ConfigureAwait(false);
            }
        }
    }

    private async Task SeedSupportUsersAsync()
    {
        var supportUsers = new[]
        {
            new SupportUserSeed("seed.admin", "Administrator", "Marian", "Velasco", "F", "Operations Administrator", "Administration", "Day", 48000m),
            new SupportUserSeed("seed.nurse", "Nurse", "Clarence", "Rosario", "M", "Senior Nurse", "Nursing", "Night", 32000m),
            new SupportUserSeed("seed.reception", "Receptionist", "Aira", "Lopez", "F", "Front Desk Officer", "Front Desk", "Day", 26000m),
            new SupportUserSeed("seed.pharmacist", "Pharmacist", "Neil", "Sarmiento", "M", "Clinical Pharmacist", "Pharmacy", "Day", 36000m),
            new SupportUserSeed("seed.labtech", "Lab Technician", "Daphne", "Mercado", "F", "Laboratory Technologist", "Laboratory", "Day", 34000m),
            new SupportUserSeed("seed.accountant", "Accountant", "Jonas", "Reyes", "M", "Accounting Officer", "Finance", "Day", 38000m),
            new SupportUserSeed("seed.hr", "HR Manager", "Karla", "Navarro", "F", "HR Manager", "Human Resources", "Day", 41000m)
        };

        foreach (var seed in supportUsers)
        {
            var roleId = _roleIds[seed.RoleName];
            if (!await ExistsAsync("SELECT 1 FROM users WHERE Username = @username LIMIT 1;", ("@username", seed.Username)).ConfigureAwait(false))
            {
                await ExecuteAsync(
                    @"INSERT INTO users (Username, PasswordHash, Email, RoleID, IsActive, LastLogin)
                      VALUES (@username, @passwordHash, @email, @roleId, 1, @lastLogin);",
                    ("@username", seed.Username),
                    ("@passwordHash", _seedPasswordHash),
                    ("@email", $"{seed.Username}@hospital.local"),
                    ("@roleId", roleId),
                    ("@lastLogin", DateTime.Now.AddDays(-2))).ConfigureAwait(false);
            }

            var userId = await ScalarAsync<int>(
                "SELECT UserID FROM users WHERE Username = @username LIMIT 1;",
                ("@username", seed.Username)).ConfigureAwait(false);
            _userIds[seed.Username] = userId;

            if (!await ExistsAsync("SELECT 1 FROM userdetails WHERE UserID = @userId LIMIT 1;", ("@userId", userId)).ConfigureAwait(false))
            {
                await ExecuteAsync(
                    @"INSERT INTO userdetails
                      (UserID, FirstName, LastName, DateOfBirth, Gender, ContactNumber, Address, EmergencyContact)
                      VALUES (@userId, @firstName, @lastName, @dob, @gender, @contact, @address, @emergency);",
                    ("@userId", userId),
                    ("@firstName", seed.FirstName),
                    ("@lastName", seed.LastName),
                    ("@dob", DateTime.Today.AddYears(-30).AddDays(userId)),
                    ("@gender", seed.Gender),
                    ("@contact", $"09{(900000000 + userId):000000000}"),
                    ("@address", $"Seed Address {seed.LastName}, Davao City"),
                    ("@emergency", $"09{(800000000 + userId):000000000}")).ConfigureAwait(false);
            }

            if (!await ExistsAsync("SELECT 1 FROM staff WHERE UserID = @userId LIMIT 1;", ("@userId", userId)).ConfigureAwait(false))
            {
                await ExecuteAsync(
                    @"INSERT INTO staff
                      (UserID, StaffCode, Designation, Department, Shift, HireDate, Salary)
                      VALUES (@userId, @staffCode, @designation, @department, @shift, @hireDate, @salary);",
                    ("@userId", userId),
                    ("@staffCode", $"STFSD{userId:000}"),
                    ("@designation", seed.Designation),
                    ("@department", seed.Department),
                    ("@shift", seed.Shift),
                    ("@hireDate", DateTime.Today.AddDays(-(90 + userId))),
                    ("@salary", seed.Salary)).ConfigureAwait(false);
            }
        }
    }

    private async Task SeedDoctorsAsync()
    {
        var doctorFirstNames = new[]
        {
            "Ramon","Liza","Edwin","Rica","Marjorie","Noel","Jessa","Arnel","Kathlyn","Paolo",
            "Irene","Victor","Grace","Alberto","Janice","Marlon","Hazel","Tyrone","Lucille","Dennis",
            "Mica","Felix","Jocelyn","Carlo","Noreen","Jerome","Patricia","Allan","Shaina","Roderick"
        };
        var doctorLastNames = new[]
        {
            "Alegre","Cabahug","Carandang","Matias","Talavera","Panganiban","Villarta","Quimpo","Bermudez","Serrano",
            "Navarro","Mercado","Aquino","David","Rosales","Dizon","Abad","Lopez","Reyes","Samonte",
            "Salazar","Domingo","Padilla","Manalo","Cabrera","Natividad","Trinidad","Ocampo","Fernandez","Lazaro"
        };

        for (var i = 1; i <= TargetCount; i++)
        {
            var username = $"seed.doctor{i:00}";
            if (!await ExistsAsync("SELECT 1 FROM users WHERE Username = @username LIMIT 1;", ("@username", username)).ConfigureAwait(false))
            {
                await ExecuteAsync(
                    @"INSERT INTO users (Username, PasswordHash, Email, RoleID, IsActive, LastLogin)
                      VALUES (@username, @passwordHash, @email, @roleId, 1, @lastLogin);",
                    ("@username", username),
                    ("@passwordHash", _seedPasswordHash),
                    ("@email", $"{username}@hospital.local"),
                    ("@roleId", _roleIds["Doctor"]),
                    ("@lastLogin", DateTime.Now.AddDays(-1))).ConfigureAwait(false);
            }

            var userId = await ScalarAsync<int>(
                "SELECT UserID FROM users WHERE Username = @username LIMIT 1;",
                ("@username", username)).ConfigureAwait(false);
            _userIds[username] = userId;

            if (!await ExistsAsync("SELECT 1 FROM userdetails WHERE UserID = @userId LIMIT 1;", ("@userId", userId)).ConfigureAwait(false))
            {
                await ExecuteAsync(
                    @"INSERT INTO userdetails
                      (UserID, FirstName, LastName, DateOfBirth, Gender, ContactNumber, Address, EmergencyContact)
                      VALUES (@userId, @firstName, @lastName, @dob, @gender, @contact, @address, @emergency);",
                    ("@userId", userId),
                    ("@firstName", doctorFirstNames[i - 1]),
                    ("@lastName", doctorLastNames[i - 1]),
                    ("@dob", DateTime.Today.AddYears(-38).AddDays(i * 110)),
                    ("@gender", i % 3 == 0 ? "F" : "M"),
                    ("@contact", $"09{(700000000 + (i * 154321)):000000000}"),
                    ("@address", $"Seed Doctor Address {i:00}, Davao Region"),
                    ("@emergency", $"09{(600000000 + (i * 123456)):000000000}")).ConfigureAwait(false);
            }

            if (!await ExistsAsync("SELECT 1 FROM staff WHERE UserID = @userId LIMIT 1;", ("@userId", userId)).ConfigureAwait(false))
            {
                await ExecuteAsync(
                    @"INSERT INTO staff
                      (UserID, StaffCode, Designation, Department, Shift, HireDate, Salary)
                      VALUES (@userId, @staffCode, @designation, @department, 'Day', @hireDate, @salary);",
                    ("@userId", userId),
                    ("@staffCode", $"STFDD{i:000}"),
                    ("@designation", "Consultant Doctor"),
                    ("@department", $"Department {i:00}"),
                    ("@hireDate", DateTime.Today.AddDays(-(420 + (i * 13)))),
                    ("@salary", 52000m + (i * 1200m))).ConfigureAwait(false);
            }

            var doctorCode = $"DOCSD{i:000}";
            if (!await ExistsAsync("SELECT 1 FROM doctors WHERE DoctorCode = @doctorCode LIMIT 1;", ("@doctorCode", doctorCode)).ConfigureAwait(false))
            {
                await ExecuteAsync(
                    @"INSERT INTO doctors
                      (UserID, DoctorCode, SpecializationID, Qualification, LicenseNumber, YearsOfExperience, ConsultationFee, IsAvailable, JoiningDate)
                      VALUES (@userId, @doctorCode, @specializationId, @qualification, @licenseNumber, @experience, @fee, 1, @joiningDate);",
                    ("@userId", userId),
                    ("@doctorCode", doctorCode),
                    ("@specializationId", _specializationIds[$"SP{i:00}"]),
                    ("@qualification", "MD, Specialty Board Certified"),
                    ("@licenseNumber", $"PRCSD{i:000}"),
                    ("@experience", 5 + i),
                    ("@fee", 1200m + (i * 75m)),
                    ("@joiningDate", DateTime.Today.AddDays(-(420 + (i * 13))))).ConfigureAwait(false);
            }

            var doctorId = await ScalarAsync<int>(
                "SELECT DoctorID FROM doctors WHERE DoctorCode = @doctorCode LIMIT 1;",
                ("@doctorCode", doctorCode)).ConfigureAwait(false);
            _doctorIds[doctorCode] = doctorId;

            if (!await ExistsAsync("SELECT 1 FROM doctorschedules WHERE DoctorID = @doctorId LIMIT 1;", ("@doctorId", doctorId)).ConfigureAwait(false))
            {
                for (var day = 1; day <= 5; day++)
                {
                    await ExecuteAsync(
                        @"INSERT INTO doctorschedules
                          (DoctorID, DayOfWeek, StartTime, EndTime, MaxAppointments, IsActive)
                          VALUES (@doctorId, @day, @startTime, @endTime, @maxAppointments, 1);",
                        ("@doctorId", doctorId),
                        ("@day", day),
                        ("@startTime", TimeSpan.FromHours(8)),
                        ("@endTime", TimeSpan.FromHours(day % 2 == 0 ? 16 : 17)),
                        ("@maxAppointments", 18 + (i % 5))).ConfigureAwait(false);
                }
            }
        }
    }

    private async Task SeedPatientsAsync()
    {
        var patientFirstNames = new[]
        {
            "Andrei","Paolo","Miguel","Jericho","Rafael","Bryan","Carlo","Nathaniel","Joshua","Kevin",
            "Alyssa","Katrina","Janelle","Camille","Patricia","Bea","Angelica","Clarisse","Mica","Joyce",
            "Hazel","Janine","Bianca","Trisha","Kaye","Erika","Dianne","Shaina","Nica","Rica"
        };
        var patientLastNames = new[]
        {
            "Dela Cruz","Santos","Reyes","Bautista","Garcia","Mendoza","Torres","Ramos","Flores","Gonzales",
            "Fernandez","Navarro","Villanueva","Aguilar","Castillo","Soriano","Domingo","Aquino","Mercado","Salazar",
            "Pascual","Valdez","Cabrera","Padilla","Lim","Tan","Abad","Rosales","Malabanan","Lopez"
        };

        for (var i = 1; i <= TargetCount; i++)
        {
            var patientCode = $"PATSD{i:000}";
            if (!await ExistsAsync("SELECT 1 FROM patients WHERE PatientCode = @code LIMIT 1;", ("@code", patientCode)).ConfigureAwait(false))
            {
                await ExecuteAsync(
                    @"INSERT INTO patients
                      (PatientCode, FirstName, LastName, DateOfBirth, Gender, BloodGroup, MaritalStatus, Nationality, IdentificationType, IdentificationNumber, RegistrationDate, IsActive)
                      VALUES (@code, @firstName, @lastName, @dob, @gender, @bloodGroup, @maritalStatus, 'Filipino', @identificationType, @identificationNumber, @registrationDate, 1);",
                    ("@code", patientCode),
                    ("@firstName", patientFirstNames[i - 1]),
                    ("@lastName", patientLastNames[i - 1]),
                    ("@dob", DateTime.Today.AddYears(-(18 + i)).AddDays(i * 20)),
                    ("@gender", i % 2 == 0 ? "F" : "M"),
                    ("@bloodGroup", BloodGroup(i)),
                    ("@maritalStatus", i % 3 == 0 ? "Married" : i % 5 == 0 ? "Widowed" : "Single"),
                    ("@identificationType", i % 2 == 0 ? "PhilSys" : "Passport"),
                    ("@identificationNumber", $"ID-SD-{i:0000}"),
                    ("@registrationDate", DateTime.Today.AddDays(-(120 - i)).AddHours(8 + (i % 7)))).ConfigureAwait(false);
            }

            var patientId = await ScalarAsync<int>(
                "SELECT PatientID FROM patients WHERE PatientCode = @code LIMIT 1;",
                ("@code", patientCode)).ConfigureAwait(false);
            _patientIds[patientCode] = patientId;

            await EnsurePatientContactAsync(patientId, "Phone", $"09{(500000000 + (i * 154321)):000000000}", true).ConfigureAwait(false);
            await EnsurePatientContactAsync(patientId, "Email", $"patient{i:000}@mail.local", false).ConfigureAwait(false);
            await EnsurePatientContactAsync(patientId, "Address", $"Lot {i}, Seed Street, Davao City", true).ConfigureAwait(false);

            if (!await ExistsAsync("SELECT 1 FROM medicalhistories WHERE PatientID = @patientId LIMIT 1;", ("@patientId", patientId)).ConfigureAwait(false))
            {
                await ExecuteAsync(
                    @"INSERT INTO medicalhistories
                      (PatientID, HistoryType, Description, DiagnosisDate, Severity, Status, RecordedBy, RecordedDate)
                      VALUES (@patientId, 'Chronic Condition', @description, @diagnosisDate, @severity, 'Active', @recordedBy, @recordedDate);",
                    ("@patientId", patientId),
                    ("@description", SeedCondition(i)),
                    ("@diagnosisDate", DateTime.Today.AddDays(-(60 + (i * 9)))),
                    ("@severity", i % 3 == 0 ? "Severe" : i % 2 == 0 ? "Moderate" : "Mild"),
                    ("@recordedBy", _userIds["seed.admin"]),
                    ("@recordedDate", DateTime.Today.AddDays(-(30 + i)))).ConfigureAwait(false);
            }
        }
    }

    private async Task EnsurePatientContactAsync(int patientId, string contactType, string value, bool isPrimary)
    {
        if (!await ExistsAsync(
                "SELECT 1 FROM patientcontacts WHERE PatientID = @patientId AND ContactType = @contactType AND ContactValue = @value LIMIT 1;",
                ("@patientId", patientId),
                ("@contactType", contactType),
                ("@value", value)).ConfigureAwait(false))
        {
            await ExecuteAsync(
                "INSERT INTO patientcontacts (PatientID, ContactType, ContactValue, IsPrimary) VALUES (@patientId, @contactType, @value, @isPrimary);",
                ("@patientId", patientId),
                ("@contactType", contactType),
                ("@value", value),
                ("@isPrimary", isPrimary)).ConfigureAwait(false);
        }
    }

    private async Task SeedAppointmentsAsync()
    {
        var receptionistId = _userIds["seed.reception"];

        for (var i = 1; i <= 45; i++)
        {
            var code = $"APTSD{i:000}";
            if (await ExistsAsync("SELECT 1 FROM appointments WHERE AppointmentCode = @code LIMIT 1;", ("@code", code)).ConfigureAwait(false))
            {
                continue;
            }

            var patientId = _patientIds[$"PATSD{(((i - 1) % TargetCount) + 1):000}"];
            var doctorId = _doctorIds[$"DOCSD{(((i - 1) % TargetCount) + 1):000}"];
            var status = i <= 30 ? "Completed" : i <= 40 ? "Scheduled" : "Cancelled";
            var date = i <= 30 ? DateTime.Today.AddDays(-(35 - i)) : DateTime.Today.AddDays(i - 30);
            var time = TimeSpan.FromHours(8 + (i % 8));

            await ExecuteAsync(
                @"INSERT INTO appointments
                  (AppointmentCode, PatientID, DoctorID, AppointmentDate, AppointmentTime, AppointmentType, Status, Reason, Duration, CreatedBy, CreatedDate, Notes)
                  VALUES (@code, @patientId, @doctorId, @appointmentDate, @appointmentTime, @appointmentType, @status, @reason, @duration, @createdBy, @createdDate, @notes);",
                ("@code", code),
                ("@patientId", patientId),
                ("@doctorId", doctorId),
                ("@appointmentDate", date.Date),
                ("@appointmentTime", time),
                ("@appointmentType", AppointmentType(i)),
                ("@status", status),
                ("@reason", $"Seeded consultation note {i:00}."),
                ("@duration", 15 + ((i % 4) * 15)),
                ("@createdBy", receptionistId),
                ("@createdDate", date.Date.AddDays(-2).Add(time)),
                ("@notes", status == "Cancelled"
                    ? "Cancelled by patient due to conflict."
                    : "Patient instructed to arrive 15 minutes early.")).ConfigureAwait(false);
        }

        _appointmentIds.Clear();
        await using (var command = CreateCommand("SELECT AppointmentCode, AppointmentID FROM appointments WHERE AppointmentCode LIKE 'APTSD%';"))
        await using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
        {
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                _appointmentIds[reader.GetString("AppointmentCode")] = reader.GetInt32("AppointmentID");
            }
        }

        for (var i = 1; i <= 45; i++)
        {
            var code = $"APTSD{i:000}";
            var appointmentId = _appointmentIds[code];
            if (!await ExistsAsync("SELECT 1 FROM appointmenthistory WHERE AppointmentID = @appointmentId LIMIT 1;", ("@appointmentId", appointmentId)).ConfigureAwait(false))
            {
                await ExecuteAsync(
                    @"INSERT INTO appointmenthistory
                      (AppointmentID, Status, ChangedBy, ChangedDate, Notes)
                      VALUES (@appointmentId, @status, @changedBy, @changedDate, @notes);",
                    ("@appointmentId", appointmentId),
                    ("@status", i <= 30 ? "Completed" : i <= 40 ? "Scheduled" : "Cancelled"),
                    ("@changedBy", receptionistId),
                    ("@changedDate", DateTime.Now.AddDays(-i)),
                    ("@notes", $"Seed history for appointment {code}.")).ConfigureAwait(false);
            }
        }
    }

    private async Task SeedVisitsAsync()
    {
        for (var i = 1; i <= TargetCount; i++)
        {
            var code = $"VISSD{i:000}";
            if (await ExistsAsync("SELECT 1 FROM visits WHERE VisitCode = @code LIMIT 1;", ("@code", code)).ConfigureAwait(false))
            {
                continue;
            }

            var appointmentCode = $"APTSD{i:000}";
            var appointmentId = _appointmentIds[appointmentCode];
            var patientId = _patientIds[$"PATSD{(((i - 1) % TargetCount) + 1):000}"];
            var doctorId = _doctorIds[$"DOCSD{(((i - 1) % TargetCount) + 1):000}"];
            var visitDate = DateTime.Today.AddDays(-(32 - i)).AddHours(10 + (i % 5));

            await ExecuteAsync(
                @"INSERT INTO visits
                  (VisitCode, PatientID, DoctorID, AppointmentID, VisitDate, Symptoms, Diagnosis, Treatment, FollowUpDate, VisitStatus, CreatedBy)
                  VALUES (@code, @patientId, @doctorId, @appointmentId, @visitDate, @symptoms, @diagnosis, @treatment, @followUpDate, 'Completed', @createdBy);",
                ("@code", code),
                ("@patientId", patientId),
                ("@doctorId", doctorId),
                ("@appointmentId", appointmentId),
                ("@visitDate", visitDate),
                ("@symptoms", $"Seed symptoms entry {i:00}"),
                ("@diagnosis", SeedCondition(i)),
                ("@treatment", $"Treatment plan {i:00} with monitoring instructions."),
                ("@followUpDate", i % 4 == 0 ? DBNull.Value : visitDate.Date.AddDays(14)),
                ("@createdBy", _userIds["seed.admin"])).ConfigureAwait(false);
        }

        _visitIds.Clear();
        await using var command = CreateCommand("SELECT VisitCode, VisitID FROM visits WHERE VisitCode LIKE 'VISSD%';");
        await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            _visitIds[reader.GetString("VisitCode")] = reader.GetInt32("VisitID");
        }
    }

    private async Task SeedPrescriptionsAsync()
    {
        for (var i = 1; i <= TargetCount; i++)
        {
            var code = $"RXSD{i:000}";
            if (!await ExistsAsync("SELECT 1 FROM prescriptions WHERE PrescriptionCode = @code LIMIT 1;", ("@code", code)).ConfigureAwait(false))
            {
                var visitCode = $"VISSD{i:000}";
                var patientId = _patientIds[$"PATSD{(((i - 1) % TargetCount) + 1):000}"];
                var doctorId = _doctorIds[$"DOCSD{(((i - 1) % TargetCount) + 1):000}"];
                await ExecuteAsync(
                    @"INSERT INTO prescriptions
                      (PrescriptionCode, VisitID, PatientID, DoctorID, PrescriptionDate, Instructions, Status)
                      VALUES (@code, @visitId, @patientId, @doctorId, @prescriptionDate, @instructions, @status);",
                    ("@code", code),
                    ("@visitId", _visitIds[visitCode]),
                    ("@patientId", patientId),
                    ("@doctorId", doctorId),
                    ("@prescriptionDate", DateTime.Today.AddDays(-(28 - i)).AddHours(11)),
                    ("@instructions", "Take medication exactly as prescribed and return for follow-up."),
                    ("@status", i % 5 == 0 ? "Completed" : "Active")).ConfigureAwait(false);
            }
        }

        for (var i = 1; i <= TargetCount; i++)
        {
            var prescriptionId = await ScalarAsync<int>(
                "SELECT PrescriptionID FROM prescriptions WHERE PrescriptionCode = @code LIMIT 1;",
                ("@code", $"RXSD{i:000}")).ConfigureAwait(false);
            if (!await ExistsAsync("SELECT 1 FROM prescriptiondetails WHERE PrescriptionID = @prescriptionId LIMIT 1;", ("@prescriptionId", prescriptionId)).ConfigureAwait(false))
            {
                await ExecuteAsync(
                    @"INSERT INTO prescriptiondetails
                      (PrescriptionID, MedicineName, Dosage, Frequency, Duration, Instructions)
                      VALUES (@prescriptionId, @medicineName, @dosage, @frequency, @duration, @instructions);",
                    ("@prescriptionId", prescriptionId),
                    ("@medicineName", $"Seed Medicine {((i - 1) % TargetCount) + 1:00}"),
                    ("@dosage", i % 3 == 0 ? "1 capsule" : "1 tablet"),
                    ("@frequency", i % 2 == 0 ? "Twice daily" : "Once daily"),
                    ("@duration", $"{5 + (i % 7)} days"),
                    ("@instructions", "Take after meals unless otherwise advised.")).ConfigureAwait(false);
            }
        }
    }

    private async Task SeedAdmissionsAsync()
    {
        for (var i = 1; i <= TargetCount; i++)
        {
            var code = $"ADMNSD{i:000}";
            if (await ExistsAsync("SELECT 1 FROM admissions WHERE AdmissionNumber = @code LIMIT 1;", ("@code", code)).ConfigureAwait(false))
            {
                continue;
            }

            var patientId = _patientIds[$"PATSD{i:000}"];
            var doctorId = _doctorIds[$"DOCSD{i:000}"];
            var roomId = _roomIds[$"RMSD{i:000}"];
            var admissionDate = DateTime.Today.AddDays(-(24 - i)).AddHours(14);
            var discharged = i <= 15;

            await ExecuteAsync(
                @"INSERT INTO admissions
                  (AdmissionNumber, PatientID, DoctorID, RoomID, AdmissionDate, ExpectedDischargeDate, ActualDischargeDate, AdmissionReason, Diagnosis, Status, DischargeSummary)
                  VALUES (@number, @patientId, @doctorId, @roomId, @admissionDate, @expectedDischargeDate, @actualDischargeDate, @reason, @diagnosis, @status, @summary);",
                ("@number", code),
                ("@patientId", patientId),
                ("@doctorId", doctorId),
                ("@roomId", roomId),
                ("@admissionDate", admissionDate),
                ("@expectedDischargeDate", admissionDate.Date.AddDays(4 + (i % 5))),
                ("@actualDischargeDate", discharged ? admissionDate.AddDays(4 + (i % 5)).AddHours(3) : DBNull.Value),
                ("@reason", "Seed inpatient monitoring after emergency consult."),
                ("@diagnosis", SeedCondition(i)),
                ("@status", discharged ? "Discharged" : "Admitted"),
                ("@summary", discharged ? "Patient stabilized and discharged with home care instructions." : DBNull.Value)).ConfigureAwait(false);
        }

        for (var i = 1; i <= TargetCount; i++)
        {
            var admissionId = await ScalarAsync<int>(
                "SELECT AdmissionID FROM admissions WHERE AdmissionNumber = @number LIMIT 1;",
                ("@number", $"ADMNSD{i:000}")).ConfigureAwait(false);
            if (!await ExistsAsync("SELECT 1 FROM bedallocations WHERE AdmissionID = @admissionId LIMIT 1;", ("@admissionId", admissionId)).ConfigureAwait(false))
            {
                await ExecuteAsync(
                    @"INSERT INTO bedallocations
                      (AdmissionID, RoomID, BedNumber, AllocationDate, DischargeDate, Status)
                      VALUES (@admissionId, @roomId, @bedNumber, @allocationDate, @dischargeDate, @status);",
                    ("@admissionId", admissionId),
                    ("@roomId", _roomIds[$"RMSD{i:000}"]),
                    ("@bedNumber", $"B-{((i - 1) % 2) + 1:00}"),
                    ("@allocationDate", DateTime.Today.AddDays(-(24 - i)).AddHours(15)),
                    ("@dischargeDate", i <= 15 ? DateTime.Today.AddDays(-(20 - i)).AddHours(12) : DBNull.Value),
                    ("@status", i <= 15 ? "Discharged" : "Occupied")).ConfigureAwait(false);
            }
        }
    }

    private async Task SeedInvoicesAsync()
    {
        for (var i = 1; i <= 40; i++)
        {
            var number = $"INVSD{i:000}";
            if (await ExistsAsync("SELECT 1 FROM invoices WHERE InvoiceNumber = @number LIMIT 1;", ("@number", number)).ConfigureAwait(false))
            {
                continue;
            }

            var appointmentCode = $"APTSD{i:000}";
            var patientId = _patientIds[$"PATSD{(((i - 1) % TargetCount) + 1):000}"];
            var totalAmount = 1800m + (i * 110m);
            var discount = i % 6 == 0 ? 180m : 0m;
            var tax = Math.Round((totalAmount - discount) * 0.12m, 2);
            var grandTotal = Math.Round((totalAmount - discount) + tax, 2);
            var status = i <= 15 ? "Paid" : i <= 30 ? "Partial" : "Pending";
            var invoiceDate = DateTime.Today.AddDays(-(30 - i)).AddHours(16);

            await ExecuteAsync(
                @"INSERT INTO invoices
                  (InvoiceNumber, PatientID, AppointmentID, InvoiceDate, DueDate, TotalAmount, Discount, TaxAmount, GrandTotal, Status, CreatedBy, Notes)
                  VALUES (@number, @patientId, @appointmentId, @invoiceDate, @dueDate, @totalAmount, @discount, @taxAmount, @grandTotal, @status, @createdBy, @notes);",
                ("@number", number),
                ("@patientId", patientId),
                ("@appointmentId", _appointmentIds[appointmentCode]),
                ("@invoiceDate", invoiceDate),
                ("@dueDate", invoiceDate.AddDays(10 + (i % 6))),
                ("@totalAmount", totalAmount),
                ("@discount", discount),
                ("@taxAmount", tax),
                ("@grandTotal", grandTotal),
                ("@status", status),
                ("@createdBy", _userIds["seed.admin"]),
                ("@notes", "Seeded invoice linked to generated appointment.")).ConfigureAwait(false);
        }

        _invoiceIds.Clear();
        await using (var command = CreateCommand("SELECT InvoiceNumber, InvoiceID FROM invoices WHERE InvoiceNumber LIKE 'INVSD%';"))
        await using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
        {
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                _invoiceIds[reader.GetString("InvoiceNumber")] = reader.GetInt32("InvoiceID");
            }
        }

        for (var i = 1; i <= 40; i++)
        {
            var invoiceId = _invoiceIds[$"INVSD{i:000}"];
            if (!await ExistsAsync("SELECT 1 FROM invoicedetails WHERE InvoiceID = @invoiceId LIMIT 1;", ("@invoiceId", invoiceId)).ConfigureAwait(false))
            {
                await ExecuteAsync(
                    @"INSERT INTO invoicedetails (InvoiceID, ServiceID, Quantity, UnitPrice)
                      VALUES (@invoiceId, @serviceId, @quantity, @unitPrice);",
                    ("@invoiceId", invoiceId),
                    ("@serviceId", _serviceIds[$"SRVSD{(((i - 1) % TargetCount) + 1):000}"]),
                    ("@quantity", 1 + (i % 3)),
                    ("@unitPrice", 750m + (i * 40m))).ConfigureAwait(false);
            }
        }

        for (var i = 1; i <= TargetCount; i++)
        {
            var paymentNumber = $"PAYSD{i:000}";
            if (await ExistsAsync("SELECT 1 FROM payments WHERE PaymentNumber = @number LIMIT 1;", ("@number", paymentNumber)).ConfigureAwait(false))
            {
                continue;
            }

            var invoiceNumber = $"INVSD{i:000}";
            var invoiceId = _invoiceIds[invoiceNumber];
            var grandTotal = await ScalarAsync<decimal>(
                "SELECT GrandTotal FROM invoices WHERE InvoiceID = @invoiceId;",
                ("@invoiceId", invoiceId)).ConfigureAwait(false);
            var status = await ScalarAsync<string>(
                "SELECT Status FROM invoices WHERE InvoiceID = @invoiceId;",
                ("@invoiceId", invoiceId)).ConfigureAwait(false);
            var amount = string.Equals(status, "Paid", StringComparison.OrdinalIgnoreCase)
                ? grandTotal
                : Math.Round(grandTotal * 0.55m, 2);

            await ExecuteAsync(
                @"INSERT INTO payments
                  (PaymentNumber, InvoiceID, PaymentDate, PaymentMethod, Amount, ReferenceNumber, ReceivedBy, Notes)
                  VALUES (@number, @invoiceId, @paymentDate, @paymentMethod, @amount, @referenceNumber, @receivedBy, @notes);",
                ("@number", paymentNumber),
                ("@invoiceId", invoiceId),
                ("@paymentDate", DateTime.Today.AddDays(-(18 - i)).AddHours(13)),
                ("@paymentMethod", i % 2 == 0 ? "Online" : "Cash"),
                ("@amount", amount),
                ("@referenceNumber", $"REFSD{i:000}"),
                ("@receivedBy", _userIds["seed.accountant"]),
                ("@notes", "Seeded payment entry.")).ConfigureAwait(false);
        }
    }

    private async Task SeedLabOrdersAsync()
    {
        for (var i = 1; i <= TargetCount; i++)
        {
            var orderCode = $"LABSD{i:000}";
            if (await ExistsAsync("SELECT 1 FROM laborders WHERE OrderCode = @code LIMIT 1;", ("@code", orderCode)).ConfigureAwait(false))
            {
                continue;
            }

            var visitId = _visitIds[$"VISSD{i:000}"];
            var patientId = _patientIds[$"PATSD{i:000}"];
            var doctorId = _doctorIds[$"DOCSD{i:000}"];
            var completed = i <= 12;
            var inProgress = i > 12 && i <= 22;

            await ExecuteAsync(
                @"INSERT INTO laborders
                  (OrderCode, VisitID, PatientID, DoctorID, OrderDate, Status, ResultDate, Notes)
                  VALUES (@code, @visitId, @patientId, @doctorId, @orderDate, @status, @resultDate, @notes);",
                ("@code", orderCode),
                ("@visitId", visitId),
                ("@patientId", patientId),
                ("@doctorId", doctorId),
                ("@orderDate", DateTime.Today.AddDays(-(16 - i)).AddHours(9)),
                ("@status", completed ? "Completed" : inProgress ? "In Progress" : "Pending"),
                ("@resultDate", completed ? DateTime.Today.AddDays(-(15 - i)).AddHours(17) : DBNull.Value),
                ("@notes", "Seeded laboratory order.")).ConfigureAwait(false);
        }

        for (var i = 1; i <= TargetCount; i++)
        {
            var orderId = await ScalarAsync<int>(
                "SELECT OrderID FROM laborders WHERE OrderCode = @code LIMIT 1;",
                ("@code", $"LABSD{i:000}")).ConfigureAwait(false);
            if (!await ExistsAsync("SELECT 1 FROM laborderdetails WHERE OrderID = @orderId LIMIT 1;", ("@orderId", orderId)).ConfigureAwait(false))
            {
                var completed = i <= 12;
                await ExecuteAsync(
                    @"INSERT INTO laborderdetails
                      (OrderID, TestID, ResultValue, ResultUnit, NormalRange, IsNormal, Notes, TechnicianID, CompletedDate)
                      VALUES (@orderId, @testId, @resultValue, @resultUnit, @normalRange, @isNormal, @notes, @technicianId, @completedDate);",
                    ("@orderId", orderId),
                    ("@testId", _labTestIds[$"LABTSD{i:000}"]),
                    ("@resultValue", completed ? (80 + (i * 3)).ToString() : DBNull.Value),
                    ("@resultUnit", completed ? "mg/dL" : DBNull.Value),
                    ("@normalRange", completed ? "70-140" : DBNull.Value),
                    ("@isNormal", completed ? (i % 4 == 0 ? 0 : 1) : DBNull.Value),
                    ("@notes", "Seeded laboratory detail result."),
                    ("@technicianId", _userIds["seed.labtech"]),
                    ("@completedDate", completed ? DateTime.Today.AddDays(-(15 - i)).AddHours(17) : DBNull.Value)).ConfigureAwait(false);
            }
        }
    }

    private async Task SeedPharmacySalesAsync()
    {
        for (var i = 1; i <= TargetCount; i++)
        {
            var saleNumber = $"SALSD{i:000}";
            if (await ExistsAsync("SELECT 1 FROM pharmacysales WHERE SaleNumber = @number LIMIT 1;", ("@number", saleNumber)).ConfigureAwait(false))
            {
                continue;
            }

            await ExecuteAsync(
                @"INSERT INTO pharmacysales
                  (SaleNumber, PatientID, SaleDate, TotalAmount, Discount, NetAmount, PaymentStatus, SoldBy)
                  VALUES (@number, @patientId, @saleDate, 0, 0, 0, @status, @soldBy);",
                ("@number", saleNumber),
                ("@patientId", _patientIds[$"PATSD{i:000}"]),
                ("@saleDate", DateTime.Today.AddDays(-(12 - i)).AddHours(15)),
                ("@status", i <= 12 ? "Paid" : i <= 22 ? "Partial" : "Pending"),
                ("@soldBy", _userIds["seed.pharmacist"])).ConfigureAwait(false);
        }

        _saleIds.Clear();
        await using (var command = CreateCommand("SELECT SaleNumber, SaleID FROM pharmacysales WHERE SaleNumber LIKE 'SALSD%';"))
        await using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
        {
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                _saleIds[reader.GetString("SaleNumber")] = reader.GetInt32("SaleID");
            }
        }

        for (var i = 1; i <= TargetCount; i++)
        {
            var saleId = _saleIds[$"SALSD{i:000}"];
            if (!await ExistsAsync("SELECT 1 FROM pharmacysaledetails WHERE SaleID = @saleId LIMIT 1;", ("@saleId", saleId)).ConfigureAwait(false))
            {
                await ExecuteAsync(
                    @"INSERT INTO pharmacysaledetails (SaleID, MedicineID, BatchNumber, Quantity, UnitPrice)
                      VALUES (@saleId, @medicineId, @batchNumber, @quantity, @unitPrice);",
                    ("@saleId", saleId),
                    ("@medicineId", _medicineIds[$"MEDSD{i:000}"]),
                    ("@batchNumber", $"PHA-SD-{i:000}-A"),
                    ("@quantity", 2 + (i % 4)),
                    ("@unitPrice", 45m + (i * 2m))).ConfigureAwait(false);

                await ExecuteAsync(
                    @"INSERT INTO pharmacysaledetails (SaleID, MedicineID, BatchNumber, Quantity, UnitPrice)
                      VALUES (@saleId, @medicineId, @batchNumber, @quantity, @unitPrice);",
                    ("@saleId", saleId),
                    ("@medicineId", _medicineIds[$"MEDSD{(((i + 5) - 1) % TargetCount) + 1:000}"]),
                    ("@batchNumber", $"PHA-SD-{i:000}-B"),
                    ("@quantity", 1 + (i % 3)),
                    ("@unitPrice", 32m + (i * 1.5m))).ConfigureAwait(false);
            }
        }

        await ExecuteAsync(
            @"UPDATE pharmacysales ps
              INNER JOIN (
                  SELECT SaleID, SUM(Quantity * UnitPrice) AS Gross
                  FROM pharmacysaledetails
                  GROUP BY SaleID
              ) agg ON agg.SaleID = ps.SaleID
              SET ps.TotalAmount = agg.Gross,
                  ps.Discount = CASE WHEN agg.Gross > 600 THEN 50 ELSE 20 END,
                  ps.NetAmount = agg.Gross - CASE WHEN agg.Gross > 600 THEN 50 ELSE 20 END
              WHERE ps.SaleNumber LIKE 'SALSD%';").ConfigureAwait(false);
    }

    private async Task SeedNotificationsAsync()
    {
        var userIdList = await LoadUserIdsOrderedAsync().ConfigureAwait(false);
        for (var i = 1; i <= TargetCount; i++)
        {
            var title = $"Seed Notification {i:00}";
            if (!await ExistsAsync("SELECT 1 FROM notifications WHERE Title = @title LIMIT 1;", ("@title", title)).ConfigureAwait(false))
            {
                var userId = userIdList[(i - 1) % userIdList.Count];
                await ExecuteAsync(
                    @"INSERT INTO notifications
                      (UserID, Title, Message, NotificationType, IsRead, CreatedDate, ExpiryDate)
                      VALUES (@userId, @title, @message, 'System', @isRead, @createdDate, @expiryDate);",
                    ("@userId", userId),
                    ("@title", title),
                    ("@message", $"Seeded system notification message {i:00}."),
                    ("@isRead", i % 2 == 0),
                    ("@createdDate", DateTime.Today.AddDays(-(10 - (i % 10))).AddHours(8 + (i % 6))),
                    ("@expiryDate", DateTime.Today.AddDays(30 + i))).ConfigureAwait(false);
            }
        }
    }

    private async Task SeedAuditLogsAsync()
    {
        var userIdList = await LoadUserIdsOrderedAsync().ConfigureAwait(false);
        for (var i = 1; i <= TargetCount; i++)
        {
            if (!await ExistsAsync("SELECT 1 FROM auditlogs WHERE Action = @action AND TableName = @tableName AND RecordID = @recordId LIMIT 1;",
                    ("@action", AuditAction(i)),
                    ("@tableName", AuditTable(i)),
                    ("@recordId", i)).ConfigureAwait(false))
            {
                var userId = userIdList[(i - 1) % userIdList.Count];
                await ExecuteAsync(
                    @"INSERT INTO auditlogs
                      (UserID, Action, TableName, RecordID, OldValue, NewValue, IPAddress, MachineName, LogDate)
                      VALUES (@userId, @action, @tableName, @recordId, NULL, NULL, @ipAddress, @machineName, @logDate);",
                    ("@userId", userId),
                    ("@action", AuditAction(i)),
                    ("@tableName", AuditTable(i)),
                    ("@recordId", i),
                    ("@ipAddress", $"192.168.10.{20 + i}"),
                    ("@machineName", $"SEED-STATION-{((i - 1) % 6) + 1}"),
                    ("@logDate", DateTime.Today.AddDays(-(20 - i)).AddHours(7 + (i % 8)))).ConfigureAwait(false);
            }
        }
    }

    private async Task RefreshBedAvailabilityAsync()
    {
        await ExecuteAsync(
            @"UPDATE rooms r
              LEFT JOIN (
                  SELECT RoomID, COUNT(*) AS OccupiedBeds
                  FROM bedallocations
                  WHERE Status = 'Occupied'
                  GROUP BY RoomID
              ) occ ON occ.RoomID = r.RoomID
              SET r.AvailableBeds = GREATEST(COALESCE(r.TotalBeds, 0) - COALESCE(occ.OccupiedBeds, 0), 0),
                  r.Status = CASE
                      WHEN COALESCE(occ.OccupiedBeds, 0) >= COALESCE(r.TotalBeds, 0) AND COALESCE(r.TotalBeds, 0) > 0 THEN 'Occupied'
                      ELSE 'Available'
                  END
              WHERE r.RoomNumber LIKE 'RMSD%';").ConfigureAwait(false);

        await ExecuteAsync(
            @"UPDATE wards w
              LEFT JOIN (
                  SELECT WardID, SUM(TotalBeds) AS TotalBeds, SUM(AvailableBeds) AS AvailableBeds
                  FROM rooms
                  GROUP BY WardID
              ) roomagg ON roomagg.WardID = w.WardID
              SET w.TotalBeds = COALESCE(roomagg.TotalBeds, w.TotalBeds),
                  w.AvailableBeds = COALESCE(roomagg.AvailableBeds, w.AvailableBeds)
              WHERE w.WardCode LIKE 'WRDSD%';").ConfigureAwait(false);
    }

    private async Task<List<int>> LoadUserIdsOrderedAsync()
    {
        var result = new List<int>();
        await using var command = CreateCommand("SELECT UserID FROM users ORDER BY UserID;");
        await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            result.Add(reader.GetInt32("UserID"));
        }

        return result;
    }

    private async Task PrintCountsAsync()
    {
        var tables = new[]
        {
            "userroles","users","userdetails","patients","patientcontacts","medicalhistories","specializations","doctors",
            "doctorschedules","staff","appointments","appointmenthistory","servicecategories","services","invoices",
            "invoicedetails","payments","visits","prescriptions","prescriptiondetails","labtests","laborders",
            "laborderdetails","medicinecategories","medicines","inventory","pharmacysales","pharmacysaledetails",
            "wards","rooms","admissions","bedallocations","systemsettings","auditlogs","notifications"
        };

        foreach (var table in tables)
        {
            var count = await ScalarAsync<int>($"SELECT COUNT(*) FROM `{table}`;").ConfigureAwait(false);
            Console.WriteLine($"{table}={count}");
        }
    }

    private MySqlCommand CreateCommand(string sql, params (string Name, object? Value)[] parameters)
    {
        var command = _connection.CreateCommand();
        if (_transaction is not null)
        {
            command.Transaction = _transaction;
        }
        command.CommandText = sql;
        foreach (var parameter in parameters)
        {
            command.Parameters.AddWithValue(parameter.Name, parameter.Value ?? DBNull.Value);
        }

        return command;
    }

    private async Task ExecuteAsync(string sql, params (string Name, object? Value)[] parameters)
    {
        await using var command = CreateCommand(sql, parameters);
        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    private async Task<bool> ExistsAsync(string sql, params (string Name, object? Value)[] parameters)
    {
        await using var command = CreateCommand(sql, parameters);
        var value = await command.ExecuteScalarAsync().ConfigureAwait(false);
        return value != null && value != DBNull.Value;
    }

    private async Task<T> ScalarAsync<T>(string sql, params (string Name, object? Value)[] parameters)
    {
        await using var command = CreateCommand(sql, parameters);
        var value = await command.ExecuteScalarAsync().ConfigureAwait(false);
        if (value == null || value == DBNull.Value)
        {
            return default!;
        }

        var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        return (T)Convert.ChangeType(value, targetType);
    }

    private static string BloodGroup(int index)
    {
        var groups = new[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
        return groups[(index - 1) % groups.Length];
    }

    private static string SeedCondition(int index)
    {
        var conditions = new[]
        {
            "Essential hypertension",
            "Type 2 diabetes mellitus",
            "Bronchial asthma",
            "Hyperlipidemia",
            "Migraine episodes",
            "Allergic rhinitis",
            "Lumbar strain",
            "Osteoarthritis",
            "Acute gastroenteritis",
            "Urinary tract infection"
        };

        return conditions[(index - 1) % conditions.Length];
    }

    private static string AppointmentType(int index)
    {
        var types = new[] { "Consultation", "Follow-up", "Emergency", "Check-up" };
        return types[(index - 1) % types.Length];
    }

    private static string AuditAction(int index)
    {
        var actions = new[] { "CREATE", "UPDATE", "VIEW", "EXPORT" };
        return actions[(index - 1) % actions.Length];
    }

    private static string AuditTable(int index)
    {
        var tables = new[] { "patients", "appointments", "invoices", "laborders", "admissions", "pharmacysales" };
        return tables[(index - 1) % tables.Length];
    }
}

internal sealed record SpecializationSeed(string Code, string Name, string Description, string Department);

internal sealed record SupportUserSeed(
    string Username,
    string RoleName,
    string FirstName,
    string LastName,
    string Gender,
    string Designation,
    string Department,
    string Shift,
    decimal Salary);
