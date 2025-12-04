using Microsoft.EntityFrameworkCore;
using Spa_Management_System.Data;
using Spa_Management_System.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Spa_Management_System.Services;

public interface ISyncService
{
    /// <summary>
    /// Get the current sync status summary
    /// </summary>
    Task<SyncStatusSummary> GetSyncStatusAsync();

    /// <summary>
    /// Perform a full sync with the cloud
    /// </summary>
    Task<SyncResult> SyncAllAsync(IProgress<SyncProgress>? progress = null);

    /// <summary>
    /// Sync only pending changes
    /// </summary>
    Task<SyncResult> SyncPendingAsync(IProgress<SyncProgress>? progress = null);

    /// <summary>
    /// Push ALL local data to cloud (initial backup)
    /// </summary>
    Task<SyncResult> PushAllDataAsync(IProgress<SyncProgress>? progress = null);

    /// <summary>
    /// Mark an entity as modified (call this when saving changes)
    /// </summary>
    void MarkAsModified<T>(T entity) where T : class, ISyncable;

    /// <summary>
    /// Get sync settings
    /// </summary>
    SyncSettings GetSettings();

    /// <summary>
    /// Update sync settings
    /// </summary>
    Task SaveSettingsAsync(SyncSettings settings);

    /// <summary>
    /// Test connection to sync server
    /// </summary>
    Task<bool> TestConnectionAsync();

    /// <summary>
    /// Get last sync time
    /// </summary>
    DateTime? GetLastSyncTime();

    /// <summary>
    /// Get cloud database row counts for verification
    /// </summary>
    Task<Dictionary<string, int>?> GetCloudCountsAsync();

    /// <summary>
    /// Verify specific records exist in the cloud
    /// </summary>
    Task<(List<long> found, List<long> missing)> VerifyRecordsAsync(string entityType, List<long> ids);

    /// <summary>
    /// Pull ALL data from cloud to local database (for new device setup)
    /// </summary>
    Task<SyncResult> PullAllDataAsync(IProgress<SyncProgress>? progress = null);
}

public class SyncService : ISyncService
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private static SyncSettings _settings = new();
    private static DateTime? _lastSyncTime;
    private static readonly string SettingsFile = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "SpaManagement", "sync_settings.json");

    public SyncService(AppDbContext context)
    {
        _context = context;
        
        // Configure HttpClient to allow HTTP (non-HTTPS) connections
        var handler = new HttpClientHandler();
#if WINDOWS
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
#endif
        _httpClient = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromMinutes(5) // 5 minutes for large data transfers
        };
        
        _jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        LoadSettings();
    }

    public SyncSettings GetSettings() => _settings;

    public DateTime? GetLastSyncTime() => _lastSyncTime;

    public async Task SaveSettingsAsync(SyncSettings settings)
    {
        _settings = settings;
        try
        {
            var directory = Path.GetDirectoryName(SettingsFile);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(SettingsFile, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save sync settings: {ex.Message}");
        }
    }

    private void LoadSettings()
    {
        try
        {
            if (File.Exists(SettingsFile))
            {
                var json = File.ReadAllText(SettingsFile);
                _settings = JsonSerializer.Deserialize<SyncSettings>(json) ?? new SyncSettings();
            }
        }
        catch
        {
            _settings = new SyncSettings();
        }
    }

    public async Task<SyncStatusSummary> GetSyncStatusAsync()
    {
        var summary = new SyncStatusSummary
        {
            LastSyncTime = _lastSyncTime,
            IsConfigured = !string.IsNullOrEmpty(_settings.ApiUrl),
            AutoSyncEnabled = _settings.AutoSyncEnabled,
            SyncIntervalMinutes = _settings.SyncIntervalMinutes
        };

        // Count pending records for each entity type
        summary.PendingCounts["Person"] = await _context.Persons.CountAsync(x => x.SyncStatus == "pending");
        summary.PendingCounts["Customer"] = await _context.Customers.CountAsync(x => x.SyncStatus == "pending");
        summary.PendingCounts["Employee"] = await _context.Employees.CountAsync(x => x.SyncStatus == "pending");
        summary.PendingCounts["Service"] = await _context.Services.CountAsync(x => x.SyncStatus == "pending");
        summary.PendingCounts["Product"] = await _context.Products.CountAsync(x => x.SyncStatus == "pending");
        summary.PendingCounts["Appointment"] = await _context.Appointments.CountAsync(x => x.SyncStatus == "pending");
        summary.PendingCounts["Sale"] = await _context.Sales.CountAsync(x => x.SyncStatus == "pending");
        summary.PendingCounts["SaleItem"] = await _context.SaleItems.CountAsync(x => x.SyncStatus == "pending");
        summary.PendingCounts["Payment"] = await _context.Payments.CountAsync(x => x.SyncStatus == "pending");
        summary.PendingCounts["Expense"] = await _context.Expenses.CountAsync(x => x.SyncStatus == "pending");
        summary.PendingCounts["Inventory"] = await _context.Inventories.CountAsync(x => x.SyncStatus == "pending");
        summary.PendingCounts["Payroll"] = await _context.Payrolls.CountAsync(x => x.SyncStatus == "pending");
        summary.PendingCounts["JournalEntry"] = await _context.JournalEntries.CountAsync(x => x.SyncStatus == "pending");
        summary.PendingCounts["JournalEntryLine"] = await _context.JournalEntryLines.CountAsync(x => x.SyncStatus == "pending");
        summary.PendingCounts["PurchaseOrder"] = await _context.PurchaseOrders.CountAsync(x => x.SyncStatus == "pending");
        summary.PendingCounts["PurchaseOrderItem"] = await _context.PurchaseOrderItems.CountAsync(x => x.SyncStatus == "pending");
        summary.PendingCounts["Supplier"] = await _context.Suppliers.CountAsync(x => x.SyncStatus == "pending");
        summary.PendingCounts["StockTransaction"] = await _context.StockTransactions.CountAsync(x => x.SyncStatus == "pending");
        summary.PendingCounts["AppointmentService"] = await _context.AppointmentServices.CountAsync(x => x.SyncStatus == "pending");

        summary.TotalPending = summary.PendingCounts.Values.Sum();

        return summary;
    }

    public async Task<bool> TestConnectionAsync()
    {
        if (string.IsNullOrEmpty(_settings.ApiUrl))
        {
            Console.WriteLine("[Sync] TestConnection: API URL is not configured");
            return false;
        }

        try
        {
            _httpClient.DefaultRequestHeaders.Clear();
            if (!string.IsNullOrEmpty(_settings.ApiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("X-API-Key", _settings.ApiKey);
            }

            Console.WriteLine($"[Sync] Testing connection to {_settings.ApiUrl}/api/data/status");
            
            // Use the status endpoint
            var response = await _httpClient.GetAsync($"{_settings.ApiUrl.TrimEnd('/')}/api/data/status");
            var content = await response.Content.ReadAsStringAsync();
            
            Console.WriteLine($"[Sync] Connection test result: {response.StatusCode} - {content}");
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[Sync] Connection test failed: HTTP {(int)response.StatusCode}");
                return false;
            }
            
            // Verify the database is connected
            try
            {
                var json = JsonSerializer.Deserialize<JsonElement>(content);
                if (json.TryGetProperty("database", out var dbProp) && dbProp.GetString() == "connected")
                {
                    Console.WriteLine("[Sync] Connection test: API online, database connected");
                    return true;
                }
                else
                {
                    Console.WriteLine("[Sync] Connection test: API online but database not connected");
                    return false;
                }
            }
            catch
            {
                return response.IsSuccessStatusCode;
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[Sync] Connection test failed: {ex.Message}");
            return false;
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("[Sync] Connection test failed: Request timed out");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Sync] Connection test error: {ex.Message}");
            return false;
        }
    }

    public async Task<Dictionary<string, int>?> GetCloudCountsAsync()
    {
        if (string.IsNullOrEmpty(_settings.ApiUrl))
            return null;

        try
        {
            _httpClient.DefaultRequestHeaders.Clear();
            if (!string.IsNullOrEmpty(_settings.ApiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("X-API-Key", _settings.ApiKey);
            }

            var response = await _httpClient.GetAsync($"{_settings.ApiUrl.TrimEnd('/')}/api/data/counts");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var json = JsonSerializer.Deserialize<JsonElement>(content);
                if (json.TryGetProperty("counts", out var countsProp))
                {
                    var counts = new Dictionary<string, int>();
                    foreach (var prop in countsProp.EnumerateObject())
                    {
                        counts[prop.Name] = prop.Value.GetInt32();
                    }
                    return counts;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Sync] Error getting cloud counts: {ex.Message}");
        }
        return null;
    }

    public async Task<(List<long> found, List<long> missing)> VerifyRecordsAsync(string entityType, List<long> ids)
    {
        if (string.IsNullOrEmpty(_settings.ApiUrl) || ids.Count == 0)
            return (new List<long>(), ids);

        try
        {
            _httpClient.DefaultRequestHeaders.Clear();
            if (!string.IsNullOrEmpty(_settings.ApiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("X-API-Key", _settings.ApiKey);
            }

            var endpoint = GetEndpointName(entityType);
            var json = JsonSerializer.Serialize(ids);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(
                $"{_settings.ApiUrl.TrimEnd('/')}/api/data/verify/{endpoint}",
                content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                
                var found = new List<long>();
                var missing = new List<long>();
                
                if (result.TryGetProperty("found", out var foundProp))
                {
                    foreach (var item in foundProp.EnumerateArray())
                    {
                        found.Add(item.GetInt64());
                    }
                }
                if (result.TryGetProperty("missing", out var missingProp))
                {
                    foreach (var item in missingProp.EnumerateArray())
                    {
                        missing.Add(item.GetInt64());
                    }
                }
                
                return (found, missing);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Sync] Error verifying records: {ex.Message}");
        }
        
        return (new List<long>(), ids);
    }

    public void MarkAsModified<T>(T entity) where T : class, ISyncable
    {
        entity.LastModifiedAt = DateTime.Now;
        entity.SyncStatus = "pending";
        entity.SyncVersion++;
    }

    public async Task<SyncResult> SyncAllAsync(IProgress<SyncProgress>? progress = null)
    {
        return await PerformSyncAsync(syncAll: true, progress);
    }

    public async Task<SyncResult> SyncPendingAsync(IProgress<SyncProgress>? progress = null)
    {
        return await PerformSyncAsync(syncAll: false, progress);
    }

    public async Task<SyncResult> PushAllDataAsync(IProgress<SyncProgress>? progress = null)
    {
        var result = new SyncResult { StartTime = DateTime.Now };

        if (string.IsNullOrEmpty(_settings.ApiUrl))
        {
            result.Success = false;
            result.ErrorMessage = "Sync is not configured. Please set the API URL in settings.";
            return result;
        }

        try
        {
            _httpClient.DefaultRequestHeaders.Clear();
            if (!string.IsNullOrEmpty(_settings.ApiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("X-API-Key", _settings.ApiKey);
            }

            // Upload ALL data in dependency order - using AsNoTracking and projecting to anonymous types to avoid navigation properties
            var uploadTasks = new (string name, Func<Task<string>> getJson)[]
            {
                ("roles", async () => JsonSerializer.Serialize(await _context.Roles.AsNoTracking().Select(r => new { r.RoleId, r.Name, r.IsArchived }).ToListAsync())),
                ("persons", async () => JsonSerializer.Serialize(await _context.Persons.AsNoTracking().Select(p => new { p.PersonId, p.SyncId, p.LastModifiedAt, p.LastSyncedAt, p.SyncStatus, p.SyncVersion, p.FirstName, p.LastName, p.Email, p.Phone, p.Address, p.Dob, p.CreatedAt }).ToListAsync())),
                ("employees", async () => JsonSerializer.Serialize(await _context.Employees.AsNoTracking().Select(e => new { e.EmployeeId, e.SyncId, e.LastModifiedAt, e.LastSyncedAt, e.SyncStatus, e.SyncVersion, e.PersonId, e.RoleId, e.HireDate, e.Status, e.Note, e.CreatedAt }).ToListAsync())),
                ("customers", async () => JsonSerializer.Serialize(await _context.Customers.AsNoTracking().Select(c => new { c.CustomerId, c.SyncId, c.LastModifiedAt, c.LastSyncedAt, c.SyncStatus, c.SyncVersion, c.PersonId, c.CustomerCode, c.LoyaltyPoints, c.CreatedAt, c.IsArchived }).ToListAsync())),
                ("servicecategories", async () => JsonSerializer.Serialize(await _context.ServiceCategories.AsNoTracking().Select(s => new { s.ServiceCategoryId, s.Name, s.Description, s.IsArchived }).ToListAsync())),
                ("services", async () => JsonSerializer.Serialize(await _context.Services.AsNoTracking().Select(s => new { s.ServiceId, s.SyncId, s.LastModifiedAt, s.LastSyncedAt, s.SyncStatus, s.SyncVersion, s.ServiceCategoryId, s.Code, s.Name, s.Description, s.Price, s.DurationMinutes }).ToListAsync())),
                ("products", async () => JsonSerializer.Serialize(await _context.Products.AsNoTracking().Select(p => new { p.ProductId, p.SyncId, p.LastModifiedAt, p.LastSyncedAt, p.SyncStatus, p.SyncVersion, p.Sku, p.Name, p.Description, p.UnitPrice, p.CostPrice, p.Unit }).ToListAsync())),
                ("inventories", async () => JsonSerializer.Serialize(await _context.Inventories.AsNoTracking().Select(i => new { i.InventoryId, i.SyncId, i.LastModifiedAt, i.LastSyncedAt, i.SyncStatus, i.SyncVersion, i.ProductId, i.QuantityOnHand, i.ReorderLevel, i.LastCountedAt }).ToListAsync())),
                ("suppliers", async () => JsonSerializer.Serialize(await _context.Suppliers.AsNoTracking().Select(s => new { s.SupplierId, s.SyncId, s.LastModifiedAt, s.LastSyncedAt, s.SyncStatus, s.SyncVersion, s.Name, s.ContactPerson, s.Phone, s.Email, s.Address, s.IsArchived }).ToListAsync())),
                ("ledgeraccounts", async () => JsonSerializer.Serialize(await _context.LedgerAccounts.AsNoTracking().Select(l => new { l.LedgerAccountId, l.Code, l.Name, l.AccountType, l.NormalBalance }).ToListAsync())),
                ("useraccounts", async () => JsonSerializer.Serialize(await _context.UserAccounts.AsNoTracking().Select(u => new { u.UserId, u.EmployeeId, u.Username, u.PasswordHash, u.IsActive, u.LastLogin, u.CreatedAt }).ToListAsync())),
                ("appointments", async () => JsonSerializer.Serialize(await _context.Appointments.AsNoTracking().Select(a => new { a.AppointmentId, a.SyncId, a.LastModifiedAt, a.LastSyncedAt, a.SyncStatus, a.SyncVersion, a.CustomerId, a.ScheduledStart, a.ScheduledEnd, a.Status, a.Notes, a.CreatedByUserId, a.CreatedAt }).ToListAsync())),
                ("appointmentservices", async () => JsonSerializer.Serialize(await _context.AppointmentServices.AsNoTracking().Select(a => new { a.ApptSrvId, a.SyncId, a.LastModifiedAt, a.LastSyncedAt, a.SyncStatus, a.SyncVersion, a.AppointmentId, a.ServiceId, a.TherapistEmployeeId, a.Price, a.CommissionAmount }).ToListAsync())),
                ("sales", async () => JsonSerializer.Serialize(await _context.Sales.AsNoTracking().Select(s => new { s.SaleId, s.SyncId, s.LastModifiedAt, s.LastSyncedAt, s.SyncStatus, s.SyncVersion, s.CustomerId, s.CreatedByUserId, s.SaleNumber, s.TotalAmount, s.PaymentStatus, s.CreatedAt }).ToListAsync())),
                ("saleitems", async () => JsonSerializer.Serialize(await _context.SaleItems.AsNoTracking().Select(s => new { s.SaleItemId, s.SyncId, s.LastModifiedAt, s.LastSyncedAt, s.SyncStatus, s.SyncVersion, s.SaleId, s.ItemType, s.ProductId, s.ServiceId, s.Qty, s.UnitPrice, s.LineTotal, s.TherapistEmployeeId }).ToListAsync())),
                ("payments", async () => JsonSerializer.Serialize(await _context.Payments.AsNoTracking().Select(p => new { p.PaymentId, p.SyncId, p.LastModifiedAt, p.LastSyncedAt, p.SyncStatus, p.SyncVersion, p.SaleId, p.PaymentMethod, p.Amount, p.PaidAt, p.RecordedByUserId }).ToListAsync())),
                ("expenses", async () => JsonSerializer.Serialize(await _context.Expenses.AsNoTracking().Select(e => new { e.ExpenseId, e.SyncId, e.LastModifiedAt, e.LastSyncedAt, e.SyncStatus, e.SyncVersion, e.ExpenseDate, e.Category, e.Description, e.Amount, e.Vendor, e.PaymentMethod, e.CreatedByUserId, e.CreatedAt }).ToListAsync())),
                ("journalentries", async () => JsonSerializer.Serialize(await _context.JournalEntries.AsNoTracking().Select(j => new { j.JournalId, j.SyncId, j.LastModifiedAt, j.LastSyncedAt, j.SyncStatus, j.SyncVersion, j.JournalNo, j.Date, j.Description, j.CreatedByUserId, j.CreatedAt }).ToListAsync())),
                ("journalentrylines", async () => JsonSerializer.Serialize(await _context.JournalEntryLines.AsNoTracking().Select(j => new { j.JournalLineId, j.SyncId, j.LastModifiedAt, j.LastSyncedAt, j.SyncStatus, j.SyncVersion, j.JournalId, j.LedgerAccountId, j.Debit, j.Credit, j.LineMemo }).ToListAsync())),
                ("payrolls", async () => JsonSerializer.Serialize(await _context.Payrolls.AsNoTracking().Select(p => new { p.PayrollId, p.SyncId, p.LastModifiedAt, p.LastSyncedAt, p.SyncStatus, p.SyncVersion, p.EmployeeId, p.PeriodStart, p.PeriodEnd, p.DaysWorked, p.DailyRate, p.GrossPay, p.Deductions, p.NetPay, p.Status, p.PaidAt, p.CreatedByUserId, p.CreatedAt }).ToListAsync())),
                ("purchaseorders", async () => JsonSerializer.Serialize(await _context.PurchaseOrders.AsNoTracking().Select(p => new { p.PoId, p.SyncId, p.LastModifiedAt, p.LastSyncedAt, p.SyncStatus, p.SyncVersion, p.PoNumber, p.SupplierId, p.Status, p.CreatedByUserId, p.CreatedAt }).ToListAsync())),
                ("purchaseorderitems", async () => JsonSerializer.Serialize(await _context.PurchaseOrderItems.AsNoTracking().Select(p => new { p.PoItemId, p.SyncId, p.LastModifiedAt, p.LastSyncedAt, p.SyncStatus, p.SyncVersion, p.PoId, p.ProductId, p.QtyOrdered, p.UnitCost }).ToListAsync())),
                ("stocktransactions", async () => JsonSerializer.Serialize(await _context.StockTransactions.AsNoTracking().Select(s => new { s.StockTxId, s.SyncId, s.LastModifiedAt, s.LastSyncedAt, s.SyncStatus, s.SyncVersion, s.ProductId, s.TxType, s.Qty, s.UnitCost, s.Reference, s.CreatedByUserId, s.CreatedAt }).ToListAsync())),
            };

            int current = 0;
            foreach (var (name, getJson) in uploadTasks)
            {
                progress?.Report(new SyncProgress
                {
                    CurrentEntity = name,
                    CurrentOperation = "Uploading",
                    PercentComplete = (int)((double)current / uploadTasks.Length * 100)
                });

                var json = await getJson();
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(
                    $"{_settings.ApiUrl.TrimEnd('/')}/api/data/upload/{name}",
                    content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadFromJsonAsync<JsonElement>();
                    if (responseData.TryGetProperty("recordsProcessed", out var countProp))
                    {
                        result.RecordsUploaded += countProp.GetInt32();
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    result.Errors.Add($"Failed to upload {name}: {response.StatusCode} - {errorContent}");
                }
                current++;
            }

            result.Success = result.Errors.Count == 0;
            result.EndTime = DateTime.Now;
            _lastSyncTime = result.EndTime;

            progress?.Report(new SyncProgress
            {
                CurrentEntity = "Complete",
                CurrentOperation = "Finished",
                PercentComplete = 100
            });
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = $"Push failed: {ex.Message}";
            result.EndTime = DateTime.Now;
        }

        return result;
    }

    public async Task<SyncResult> PullAllDataAsync(IProgress<SyncProgress>? progress = null)
    {
        var result = new SyncResult { StartTime = DateTime.Now };

        if (string.IsNullOrEmpty(_settings.ApiUrl))
        {
            result.Success = false;
            result.ErrorMessage = "Sync is not configured. Please set the API URL in settings.";
            return result;
        }

        try
        {
            _httpClient.DefaultRequestHeaders.Clear();
            if (!string.IsNullOrEmpty(_settings.ApiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("X-API-Key", _settings.ApiKey);
            }

            // Download in dependency order - parent tables first
            var downloadOrder = new[]
            {
                ("roles", "Role"),
                ("persons", "Person"),
                ("servicecategories", "ServiceCategory"),
                ("ledgeraccounts", "LedgerAccount"),
                ("employees", "Employee"),
                ("customers", "Customer"),
                ("useraccounts", "UserAccount"),
                ("services", "Service"),
                ("products", "Product"),
                ("suppliers", "Supplier"),
                ("inventories", "Inventory"),
                ("appointments", "Appointment"),
                ("appointmentservices", "AppointmentService"),
                ("sales", "Sale"),
                ("saleitems", "SaleItem"),
                ("payments", "Payment"),
                ("expenses", "Expense"),
                ("journalentries", "JournalEntry"),
                ("journalentrylines", "JournalEntryLine"),
                ("payrolls", "Payroll"),
                ("purchaseorders", "PurchaseOrder"),
                ("purchaseorderitems", "PurchaseOrderItem"),
                ("stocktransactions", "StockTransaction"),
            };

            int current = 0;
            foreach (var (endpoint, tableName) in downloadOrder)
            {
                progress?.Report(new SyncProgress
                {
                    CurrentEntity = tableName,
                    CurrentOperation = "Downloading",
                    PercentComplete = (int)((double)current / downloadOrder.Length * 100)
                });

                try
                {
                    var response = await _httpClient.GetAsync(
                        $"{_settings.ApiUrl.TrimEnd('/')}/api/data/download/{endpoint}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var json = JsonSerializer.Deserialize<JsonElement>(content);
                        
                        if (json.TryGetProperty("data", out var dataArray) && dataArray.ValueKind == JsonValueKind.Array)
                        {
                            var count = await InsertDownloadedDataAsync(tableName, dataArray);
                            result.RecordsDownloaded += count;
                            Console.WriteLine($"[Sync] Downloaded {count} {tableName} records");
                        }
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        result.Errors.Add($"Failed to download {tableName}: {response.StatusCode} - {errorContent}");
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Error downloading {tableName}: {ex.Message}");
                    Console.WriteLine($"[Sync] Error downloading {tableName}: {ex}");
                }

                current++;
            }

            result.Success = result.Errors.Count == 0;
            result.EndTime = DateTime.Now;
            _lastSyncTime = result.EndTime;

            progress?.Report(new SyncProgress
            {
                CurrentEntity = "Complete",
                CurrentOperation = "Finished",
                PercentComplete = 100
            });
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = $"Pull failed: {ex.Message}";
            result.EndTime = DateTime.Now;
        }

        return result;
    }

    private async Task<int> InsertDownloadedDataAsync(string tableName, JsonElement dataArray)
    {
        int inserted = 0;
        var connectionString = _context.Database.GetConnectionString();
        
        if (string.IsNullOrEmpty(connectionString))
        {
            Console.WriteLine($"[Sync] No connection string available");
            return 0;
        }

        using var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
        await connection.OpenAsync();

        // Get column info for the table
        var columns = await GetTableColumnsAsync(connection, tableName);
        var pkColumn = GetPrimaryKeyColumnName(tableName);
        
        foreach (var item in dataArray.EnumerateArray())
        {
            try
            {
                await UpsertWithRawSqlAsync(connection, tableName, item, columns, pkColumn);
                inserted++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Sync] Error inserting {tableName} record: {ex.Message}");
            }
        }

        return inserted;
    }

    private async Task<List<string>> GetTableColumnsAsync(Microsoft.Data.SqlClient.SqlConnection connection, string tableName)
    {
        var columns = new List<string>();
        var sql = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName";
        using var cmd = new Microsoft.Data.SqlClient.SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@tableName", tableName);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            columns.Add(reader.GetString(0).ToLower());
        }
        return columns;
    }

    private string GetPrimaryKeyColumnName(string tableName)
    {
        return tableName switch
        {
            "Role" => "role_id",
            "Person" => "person_id",
            "Employee" => "employee_id",
            "Customer" => "customer_id",
            "UserAccount" => "user_id",
            "ServiceCategory" => "service_category_id",
            "Service" => "service_id",
            "Product" => "product_id",
            "Inventory" => "inventory_id",
            "Supplier" => "supplier_id",
            "LedgerAccount" => "account_id",
            "Appointment" => "appointment_id",
            "AppointmentService" => "appt_srv_id",
            "Sale" => "sale_id",
            "SaleItem" => "sale_item_id",
            "Payment" => "payment_id",
            "Expense" => "expense_id",
            "JournalEntry" => "journal_id",
            "JournalEntryLine" => "journal_line_id",
            "Payroll" => "payroll_id",
            "PurchaseOrder" => "po_id",
            "PurchaseOrderItem" => "po_item_id",
            "StockTransaction" => "stock_tx_id",
            "EmployeeServiceCommission" => "commission_id",
            "EmployeeAttendance" => "attendance_id",
            "CrmNote" => "note_id",
            "AuditLog" => "log_id",
            _ => "id"
        };
    }

    private async Task UpsertWithRawSqlAsync(Microsoft.Data.SqlClient.SqlConnection connection, string tableName, JsonElement item, List<string> tableColumns, string pkColumn)
    {
        var columnValues = new Dictionary<string, object?>();
        
        foreach (var prop in item.EnumerateObject())
        {
            var colName = prop.Name.ToLower();
            if (!tableColumns.Contains(colName)) continue;
            
            object? value = prop.Value.ValueKind switch
            {
                JsonValueKind.Null => DBNull.Value,
                JsonValueKind.String => prop.Value.GetString(),
                JsonValueKind.Number => prop.Value.TryGetInt64(out var l) ? l : prop.Value.GetDecimal(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => prop.Value.GetRawText()
            };
            
            columnValues[colName] = value;
        }

        // Ensure sync_id is never null or empty - generate new GUID if needed
        if (tableColumns.Contains("sync_id"))
        {
            if (!columnValues.ContainsKey("sync_id") || 
                columnValues["sync_id"] == DBNull.Value || 
                columnValues["sync_id"] == null ||
                string.IsNullOrWhiteSpace(columnValues["sync_id"]?.ToString()) ||
                columnValues["sync_id"]?.ToString() == "00000000-0000-0000-0000-000000000000")
            {
                columnValues["sync_id"] = Guid.NewGuid().ToString();
            }
        }

        // Ensure sync_status has a value
        if (tableColumns.Contains("sync_status"))
        {
            if (!columnValues.ContainsKey("sync_status") || 
                columnValues["sync_status"] == DBNull.Value || 
                string.IsNullOrWhiteSpace(columnValues["sync_status"]?.ToString()))
            {
                columnValues["sync_status"] = "synced";
            }
        }

        // Ensure sync_version has a value
        if (tableColumns.Contains("sync_version"))
        {
            if (!columnValues.ContainsKey("sync_version") || columnValues["sync_version"] == DBNull.Value)
            {
                columnValues["sync_version"] = 1;
            }
        }

        if (!columnValues.ContainsKey(pkColumn))
        {
            Console.WriteLine($"[Sync] Missing primary key {pkColumn} for {tableName}");
            return;
        }

        var pkValue = columnValues[pkColumn];

        var checkSql = $"SELECT COUNT(*) FROM [{tableName}] WHERE [{pkColumn}] = @pkValue";
        using var checkCmd = new Microsoft.Data.SqlClient.SqlCommand(checkSql, connection);
        checkCmd.Parameters.AddWithValue("@pkValue", pkValue ?? DBNull.Value);
        var scalarResult = await checkCmd.ExecuteScalarAsync();
        var exists = scalarResult != null && (int)scalarResult > 0;

        if (exists)
        {
            var setClauses = columnValues.Where(kv => kv.Key != pkColumn)
                .Select((kv, i) => $"[{kv.Key}] = @p{i}").ToList();
            
            if (setClauses.Count == 0) return;
            
            var updateSql = $"UPDATE [{tableName}] SET {string.Join(", ", setClauses)} WHERE [{pkColumn}] = @pkValue";
            using var updateCmd = new Microsoft.Data.SqlClient.SqlCommand(updateSql, connection);
            
            int i = 0;
            foreach (var kv in columnValues.Where(kv => kv.Key != pkColumn))
            {
                updateCmd.Parameters.AddWithValue($"@p{i}", kv.Value ?? DBNull.Value);
                i++;
            }
            updateCmd.Parameters.AddWithValue("@pkValue", pkValue ?? DBNull.Value);
            
            await updateCmd.ExecuteNonQueryAsync();
        }
        else
        {
            var columnNames = string.Join(", ", columnValues.Keys.Select(c => $"[{c}]"));
            var paramNames = string.Join(", ", columnValues.Keys.Select((_, i) => $"@p{i}"));
            
            var insertSql = $@"
                SET IDENTITY_INSERT [{tableName}] ON;
                INSERT INTO [{tableName}] ({columnNames}) VALUES ({paramNames});
                SET IDENTITY_INSERT [{tableName}] OFF;";
            
            using var insertCmd = new Microsoft.Data.SqlClient.SqlCommand(insertSql, connection);
            
            int i = 0;
            foreach (var kv in columnValues)
            {
                insertCmd.Parameters.AddWithValue($"@p{i}", kv.Value ?? DBNull.Value);
                i++;
            }
            
            await insertCmd.ExecuteNonQueryAsync();
        }
    }

    // Keep old upsert methods for EF-based operations (not used for pull anymore)
    private async Task UpsertRoleAsync(JsonElement item)
    {
        var id = (short)item.GetProperty("role_id").GetInt32();
        var entity = await _context.Roles.FindAsync(id);
        if (entity == null)
        {
            entity = new Role { RoleId = id };
            _context.Roles.Add(entity);
        }
        entity.Name = item.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "";
        entity.IsArchived = item.TryGetProperty("is_archived", out var a) && a.GetBoolean();
    }

    private async Task UpsertPersonAsync(JsonElement item)
    {
        var id = item.GetProperty("person_id").GetInt64();
        var entity = await _context.Persons.FindAsync(id);
        if (entity == null)
        {
            entity = new Person { PersonId = id };
            _context.Persons.Add(entity);
        }
        entity.FirstName = item.TryGetProperty("first_name", out var fn) ? fn.GetString() ?? "" : "";
        entity.LastName = item.TryGetProperty("last_name", out var ln) ? ln.GetString() ?? "" : "";
        entity.Email = item.TryGetProperty("email", out var e) ? e.GetString() : null;
        entity.Phone = item.TryGetProperty("phone", out var p) ? p.GetString() : null;
        entity.Address = item.TryGetProperty("address", out var addr) ? addr.GetString() : null;
        entity.Dob = item.TryGetProperty("dob", out var d) && d.ValueKind != JsonValueKind.Null ? d.GetDateTime() : null;
        entity.CreatedAt = item.TryGetProperty("created_at", out var c) ? c.GetDateTime() : DateTime.Now;
        SetSyncProperties(entity, item);
    }

    private async Task UpsertEmployeeAsync(JsonElement item)
    {
        var id = item.GetProperty("employee_id").GetInt64();
        var entity = await _context.Employees.FindAsync(id);
        if (entity == null)
        {
            entity = new Employee { EmployeeId = id };
            _context.Employees.Add(entity);
        }
        entity.PersonId = item.GetProperty("person_id").GetInt64();
        entity.RoleId = item.TryGetProperty("role_id", out var r) && r.ValueKind != JsonValueKind.Null ? (short)r.GetInt32() : (short)1;
        entity.HireDate = item.TryGetProperty("hire_date", out var h) && h.ValueKind != JsonValueKind.Null ? h.GetDateTime() : null;
        entity.Status = item.TryGetProperty("status", out var s) ? s.GetString() ?? "active" : "active";
        entity.Note = item.TryGetProperty("note", out var n) ? n.GetString() : null;
        entity.CreatedAt = item.TryGetProperty("created_at", out var c) ? c.GetDateTime() : DateTime.Now;
        SetSyncProperties(entity, item);
    }

    private async Task UpsertCustomerAsync(JsonElement item)
    {
        var id = item.GetProperty("customer_id").GetInt64();
        var entity = await _context.Customers.FindAsync(id);
        if (entity == null)
        {
            entity = new Customer { CustomerId = id };
            _context.Customers.Add(entity);
        }
        entity.PersonId = item.GetProperty("person_id").GetInt64();
        entity.CustomerCode = item.TryGetProperty("customer_code", out var cc) ? cc.GetString() : null;
        entity.LoyaltyPoints = item.TryGetProperty("loyalty_points", out var lp) ? lp.GetInt32() : 0;
        entity.CreatedAt = item.TryGetProperty("created_at", out var c) ? c.GetDateTime() : DateTime.Now;
        entity.IsArchived = item.TryGetProperty("is_archived", out var a) && a.GetBoolean();
        SetSyncProperties(entity, item);
    }

    private async Task UpsertUserAccountAsync(JsonElement item)
    {
        var id = item.GetProperty("user_id").GetInt64();
        var entity = await _context.UserAccounts.FindAsync(id);
        if (entity == null)
        {
            entity = new UserAccount { UserId = id };
            _context.UserAccounts.Add(entity);
        }
        entity.EmployeeId = item.TryGetProperty("employee_id", out var e) && e.ValueKind != JsonValueKind.Null ? e.GetInt64() : null;
        entity.Username = item.TryGetProperty("username", out var u) ? u.GetString() ?? "" : "";
        entity.PasswordHash = item.TryGetProperty("password_hash", out var p) ? p.GetString() ?? "" : "";
        entity.IsActive = item.TryGetProperty("is_active", out var a) && a.GetBoolean();
        entity.LastLogin = item.TryGetProperty("last_login", out var l) && l.ValueKind != JsonValueKind.Null ? l.GetDateTime() : null;
        entity.CreatedAt = item.TryGetProperty("created_at", out var c) ? c.GetDateTime() : DateTime.Now;
    }

    private async Task UpsertServiceCategoryAsync(JsonElement item)
    {
        var id = item.GetProperty("service_category_id").GetInt32();
        var entity = await _context.ServiceCategories.FindAsync(id);
        if (entity == null)
        {
            entity = new ServiceCategory { ServiceCategoryId = id };
            _context.ServiceCategories.Add(entity);
        }
        entity.Name = item.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "";
        entity.Description = item.TryGetProperty("description", out var d) ? d.GetString() : null;
        entity.IsArchived = item.TryGetProperty("is_archived", out var a) && a.GetBoolean();
    }

    private async Task UpsertServiceAsync(JsonElement item)
    {
        var id = item.GetProperty("service_id").GetInt64();
        var entity = await _context.Services.FindAsync(id);
        if (entity == null)
        {
            entity = new Service { ServiceId = id };
            _context.Services.Add(entity);
        }
        entity.ServiceCategoryId = item.TryGetProperty("service_category_id", out var sc) && sc.ValueKind != JsonValueKind.Null ? (int?)sc.GetInt32() : null;
        entity.Code = item.TryGetProperty("code", out var c) ? c.GetString() : null;
        entity.Name = item.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "";
        entity.Description = item.TryGetProperty("description", out var d) ? d.GetString() : null;
        entity.Price = item.TryGetProperty("price", out var p) ? p.GetDecimal() : 0;
        entity.DurationMinutes = item.TryGetProperty("duration_minutes", out var dm) ? dm.GetInt32() : 60;
        SetSyncProperties(entity, item);
    }

    private async Task UpsertProductAsync(JsonElement item)
    {
        var id = item.GetProperty("product_id").GetInt64();
        var entity = await _context.Products.FindAsync(id);
        if (entity == null)
        {
            entity = new Product { ProductId = id };
            _context.Products.Add(entity);
        }
        entity.Sku = item.TryGetProperty("sku", out var s) ? s.GetString() : null;
        entity.Name = item.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "";
        entity.Description = item.TryGetProperty("description", out var d) ? d.GetString() : null;
        entity.UnitPrice = item.TryGetProperty("unit_price", out var up) ? up.GetDecimal() : 0;
        entity.CostPrice = item.TryGetProperty("cost_price", out var cp) ? cp.GetDecimal() : 0;
        entity.Unit = item.TryGetProperty("unit", out var u) ? u.GetString() : null;
        SetSyncProperties(entity, item);
    }

    private async Task UpsertInventoryAsync(JsonElement item)
    {
        var id = item.GetProperty("inventory_id").GetInt64();
        var entity = await _context.Inventories.FindAsync(id);
        if (entity == null)
        {
            entity = new Inventory { InventoryId = id };
            _context.Inventories.Add(entity);
        }
        entity.ProductId = item.GetProperty("product_id").GetInt64();
        entity.QuantityOnHand = item.TryGetProperty("quantity_on_hand", out var q) ? q.GetDecimal() : 0;
        entity.ReorderLevel = item.TryGetProperty("reorder_level", out var r) ? r.GetDecimal() : 0;
        entity.LastCountedAt = item.TryGetProperty("last_counted_at", out var l) && l.ValueKind != JsonValueKind.Null ? l.GetDateTime() : null;
        SetSyncProperties(entity, item);
    }

    private async Task UpsertSupplierAsync(JsonElement item)
    {
        var id = item.GetProperty("supplier_id").GetInt64();
        var entity = await _context.Suppliers.FindAsync(id);
        if (entity == null)
        {
            entity = new Supplier { SupplierId = id };
            _context.Suppliers.Add(entity);
        }
        entity.Name = item.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "";
        entity.ContactPerson = item.TryGetProperty("contact_person", out var cp) ? cp.GetString() : null;
        entity.Phone = item.TryGetProperty("phone", out var p) ? p.GetString() : null;
        entity.Email = item.TryGetProperty("email", out var e) ? e.GetString() : null;
        entity.Address = item.TryGetProperty("address", out var a) ? a.GetString() : null;
        entity.IsArchived = item.TryGetProperty("is_archived", out var ar) && ar.GetBoolean();
        SetSyncProperties(entity, item);
    }

    private async Task UpsertLedgerAccountAsync(JsonElement item)
    {
        var id = item.GetProperty("ledger_account_id").GetInt64();
        var entity = await _context.LedgerAccounts.FindAsync(id);
        if (entity == null)
        {
            entity = new LedgerAccount { LedgerAccountId = id };
            _context.LedgerAccounts.Add(entity);
        }
        entity.Code = item.TryGetProperty("code", out var c) ? c.GetString() ?? "" : "";
        entity.Name = item.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "";
        entity.AccountType = item.TryGetProperty("account_type", out var at) ? at.GetString() ?? "" : "";
        entity.NormalBalance = item.TryGetProperty("normal_balance", out var nb) ? nb.GetString() ?? "debit" : "debit";
    }

    private async Task UpsertAppointmentAsync(JsonElement item)
    {
        var id = item.GetProperty("appointment_id").GetInt64();
        var entity = await _context.Appointments.FindAsync(id);
        if (entity == null)
        {
            entity = new Appointment { AppointmentId = id };
            _context.Appointments.Add(entity);
        }
        entity.CustomerId = item.TryGetProperty("customer_id", out var c) && c.ValueKind != JsonValueKind.Null ? c.GetInt64() : 0;
        entity.ScheduledStart = item.GetProperty("scheduled_start").GetDateTime();
        entity.ScheduledEnd = item.TryGetProperty("scheduled_end", out var se) && se.ValueKind != JsonValueKind.Null ? se.GetDateTime() : null;
        entity.Status = item.TryGetProperty("status", out var s) ? s.GetString() ?? "scheduled" : "scheduled";
        entity.Notes = item.TryGetProperty("notes", out var n) ? n.GetString() : null;
        entity.CreatedByUserId = item.TryGetProperty("created_by_user_id", out var cb) && cb.ValueKind != JsonValueKind.Null ? cb.GetInt64() : null;
        entity.CreatedAt = item.TryGetProperty("created_at", out var ca) ? ca.GetDateTime() : DateTime.Now;
        SetSyncProperties(entity, item);
    }

    private async Task UpsertAppointmentServiceAsync(JsonElement item)
    {
        var id = item.GetProperty("appt_srv_id").GetInt64();
        var entity = await _context.AppointmentServices.FindAsync(id);
        if (entity == null)
        {
            entity = new AppointmentService { ApptSrvId = id };
            _context.AppointmentServices.Add(entity);
        }
        entity.AppointmentId = item.GetProperty("appointment_id").GetInt64();
        entity.ServiceId = item.GetProperty("service_id").GetInt64();
        entity.TherapistEmployeeId = item.TryGetProperty("therapist_employee_id", out var t) && t.ValueKind != JsonValueKind.Null ? t.GetInt64() : null;
        entity.Price = item.TryGetProperty("price", out var p) ? p.GetDecimal() : 0;
        entity.CommissionAmount = item.TryGetProperty("commission_amount", out var ca) ? ca.GetDecimal() : 0;
        SetSyncProperties(entity, item);
    }

    private async Task UpsertSaleAsync(JsonElement item)
    {
        var id = item.GetProperty("sale_id").GetInt64();
        var entity = await _context.Sales.FindAsync(id);
        if (entity == null)
        {
            entity = new Sale { SaleId = id };
            _context.Sales.Add(entity);
        }
        entity.CustomerId = item.TryGetProperty("customer_id", out var c) && c.ValueKind != JsonValueKind.Null ? c.GetInt64() : null;
        entity.CreatedByUserId = item.TryGetProperty("created_by_user_id", out var cb) && cb.ValueKind != JsonValueKind.Null ? cb.GetInt64() : null;
        entity.SaleNumber = item.TryGetProperty("sale_number", out var sn) ? sn.GetString() : null;
        entity.TotalAmount = item.TryGetProperty("total_amount", out var ta) ? ta.GetDecimal() : 0;
        entity.PaymentStatus = item.TryGetProperty("payment_status", out var ps) ? ps.GetString() ?? "unpaid" : "unpaid";
        entity.CreatedAt = item.TryGetProperty("created_at", out var ca) ? ca.GetDateTime() : DateTime.Now;
        SetSyncProperties(entity, item);
    }

    private async Task UpsertSaleItemAsync(JsonElement item)
    {
        var id = item.GetProperty("sale_item_id").GetInt64();
        var entity = await _context.SaleItems.FindAsync(id);
        if (entity == null)
        {
            entity = new SaleItem { SaleItemId = id };
            _context.SaleItems.Add(entity);
        }
        entity.SaleId = item.GetProperty("sale_id").GetInt64();
        entity.ItemType = item.TryGetProperty("item_type", out var it) ? it.GetString() ?? "" : "";
        entity.ProductId = item.TryGetProperty("product_id", out var p) && p.ValueKind != JsonValueKind.Null ? p.GetInt64() : null;
        entity.ServiceId = item.TryGetProperty("service_id", out var s) && s.ValueKind != JsonValueKind.Null ? s.GetInt64() : null;
        entity.Qty = item.TryGetProperty("qty", out var q) ? q.GetDecimal() : 1;
        entity.UnitPrice = item.TryGetProperty("unit_price", out var up) ? up.GetDecimal() : 0;
        entity.LineTotal = item.TryGetProperty("line_total", out var lt) ? lt.GetDecimal() : 0;
        entity.TherapistEmployeeId = item.TryGetProperty("therapist_employee_id", out var t) && t.ValueKind != JsonValueKind.Null ? t.GetInt64() : null;
        SetSyncProperties(entity, item);
    }

    private async Task UpsertPaymentAsync(JsonElement item)
    {
        var id = item.GetProperty("payment_id").GetInt64();
        var entity = await _context.Payments.FindAsync(id);
        if (entity == null)
        {
            entity = new Payment { PaymentId = id };
            _context.Payments.Add(entity);
        }
        entity.SaleId = item.TryGetProperty("sale_id", out var s) && s.ValueKind != JsonValueKind.Null ? s.GetInt64() : 0;
        entity.PaymentMethod = item.TryGetProperty("payment_method", out var pm) ? pm.GetString() ?? "cash" : "cash";
        entity.Amount = item.TryGetProperty("amount", out var a) ? a.GetDecimal() : 0;
        entity.PaidAt = item.TryGetProperty("paid_at", out var p) ? p.GetDateTime() : DateTime.Now;
        entity.RecordedByUserId = item.TryGetProperty("recorded_by_user_id", out var r) && r.ValueKind != JsonValueKind.Null ? r.GetInt64() : null;
        SetSyncProperties(entity, item);
    }

    private async Task UpsertExpenseAsync(JsonElement item)
    {
        var id = item.GetProperty("expense_id").GetInt64();
        var entity = await _context.Expenses.FindAsync(id);
        if (entity == null)
        {
            entity = new Expense { ExpenseId = id };
            _context.Expenses.Add(entity);
        }
        entity.ExpenseDate = item.TryGetProperty("expense_date", out var ed) ? ed.GetDateTime() : DateTime.Now;
        entity.Category = item.TryGetProperty("category", out var c) && c.ValueKind != JsonValueKind.Null ? c.GetString() ?? string.Empty : string.Empty;
        entity.Description = item.TryGetProperty("description", out var d) && d.ValueKind != JsonValueKind.Null ? d.GetString() ?? string.Empty : string.Empty;
        entity.Amount = item.TryGetProperty("amount", out var a) ? a.GetDecimal() : 0;
        entity.Vendor = item.TryGetProperty("vendor", out var v) && v.ValueKind != JsonValueKind.Null ? v.GetString() : null;
        entity.PaymentMethod = item.TryGetProperty("payment_method", out var pm) && pm.ValueKind != JsonValueKind.Null ? pm.GetString() ?? "Cash" : "Cash";
        entity.CreatedByUserId = item.TryGetProperty("created_by_user_id", out var cb) && cb.ValueKind != JsonValueKind.Null ? cb.GetInt64() : null;
        entity.CreatedAt = item.TryGetProperty("created_at", out var ca) ? ca.GetDateTime() : DateTime.Now;
        SetSyncProperties(entity, item);
    }

    private async Task UpsertJournalEntryAsync(JsonElement item)
    {
        var id = item.GetProperty("journal_id").GetInt64();
        var entity = await _context.JournalEntries.FindAsync(id);
        if (entity == null)
        {
            entity = new JournalEntry { JournalId = id };
            _context.JournalEntries.Add(entity);
        }
        entity.JournalNo = item.TryGetProperty("journal_no", out var jn) ? jn.GetString() : null;
        entity.Date = item.TryGetProperty("date", out var d) ? d.GetDateTime() : DateTime.Now;
        entity.Description = item.TryGetProperty("description", out var desc) ? desc.GetString() : null;
        entity.CreatedByUserId = item.TryGetProperty("created_by_user_id", out var cb) && cb.ValueKind != JsonValueKind.Null ? cb.GetInt64() : null;
        entity.CreatedAt = item.TryGetProperty("created_at", out var ca) ? ca.GetDateTime() : DateTime.Now;
        SetSyncProperties(entity, item);
    }

    private async Task UpsertJournalEntryLineAsync(JsonElement item)
    {
        var id = item.GetProperty("journal_line_id").GetInt64();
        var entity = await _context.JournalEntryLines.FindAsync(id);
        if (entity == null)
        {
            entity = new JournalEntryLine { JournalLineId = id };
            _context.JournalEntryLines.Add(entity);
        }
        entity.JournalId = item.GetProperty("journal_id").GetInt64();
        entity.LedgerAccountId = item.GetProperty("ledger_account_id").GetInt64();
        entity.Debit = item.TryGetProperty("debit", out var d) ? d.GetDecimal() : 0;
        entity.Credit = item.TryGetProperty("credit", out var c) ? c.GetDecimal() : 0;
        entity.LineMemo = item.TryGetProperty("line_memo", out var lm) ? lm.GetString() : null;
        SetSyncProperties(entity, item);
    }

    private async Task UpsertPayrollAsync(JsonElement item)
    {
        var id = item.GetProperty("payroll_id").GetInt64();
        var entity = await _context.Payrolls.FindAsync(id);
        if (entity == null)
        {
            entity = new Payroll { PayrollId = id };
            _context.Payrolls.Add(entity);
        }
        entity.EmployeeId = item.GetProperty("employee_id").GetInt64();
        entity.PeriodStart = item.GetProperty("period_start").GetDateTime();
        entity.PeriodEnd = item.GetProperty("period_end").GetDateTime();
        entity.DaysWorked = item.TryGetProperty("days_worked", out var dw) ? dw.GetInt32() : 0;
        entity.DailyRate = item.TryGetProperty("daily_rate", out var dr) ? dr.GetDecimal() : 0;
        entity.GrossPay = item.TryGetProperty("gross_pay", out var gp) ? gp.GetDecimal() : 0;
        entity.Deductions = item.TryGetProperty("deductions", out var ded) ? ded.GetDecimal() : 0;
        entity.NetPay = item.TryGetProperty("net_pay", out var np) ? np.GetDecimal() : 0;
        entity.Status = item.TryGetProperty("status", out var s) ? s.GetString() ?? "draft" : "draft";
        entity.PaidAt = item.TryGetProperty("paid_at", out var pa) && pa.ValueKind != JsonValueKind.Null ? pa.GetDateTime() : null;
        entity.CreatedByUserId = item.TryGetProperty("created_by_user_id", out var cb) && cb.ValueKind != JsonValueKind.Null ? cb.GetInt64() : null;
        entity.CreatedAt = item.TryGetProperty("created_at", out var ca) ? ca.GetDateTime() : DateTime.Now;
        SetSyncProperties(entity, item);
    }

    private async Task UpsertPurchaseOrderAsync(JsonElement item)
    {
        var id = item.GetProperty("po_id").GetInt64();
        var entity = await _context.PurchaseOrders.FindAsync(id);
        if (entity == null)
        {
            entity = new PurchaseOrder { PoId = id };
            _context.PurchaseOrders.Add(entity);
        }
        entity.SupplierId = item.GetProperty("supplier_id").GetInt64();
        entity.PoNumber = item.TryGetProperty("po_number", out var pn) ? pn.GetString() : null;
        entity.Status = item.TryGetProperty("status", out var s) ? s.GetString() : null;
        entity.CreatedByUserId = item.TryGetProperty("created_by_user_id", out var cb) && cb.ValueKind != JsonValueKind.Null ? cb.GetInt64() : null;
        entity.CreatedAt = item.TryGetProperty("created_at", out var ca) ? ca.GetDateTime() : DateTime.Now;
        SetSyncProperties(entity, item);
    }

    private async Task UpsertPurchaseOrderItemAsync(JsonElement item)
    {
        var id = item.GetProperty("po_item_id").GetInt64();
        var entity = await _context.PurchaseOrderItems.FindAsync(id);
        if (entity == null)
        {
            entity = new PurchaseOrderItem { PoItemId = id };
            _context.PurchaseOrderItems.Add(entity);
        }
        entity.PoId = item.GetProperty("po_id").GetInt64();
        entity.ProductId = item.GetProperty("product_id").GetInt64();
        entity.QtyOrdered = item.TryGetProperty("qty_ordered", out var q) ? q.GetDecimal() : 0;
        entity.UnitCost = item.TryGetProperty("unit_cost", out var uc) ? uc.GetDecimal() : 0;
        SetSyncProperties(entity, item);
    }

    private async Task UpsertStockTransactionAsync(JsonElement item)
    {
        var id = item.GetProperty("stock_tx_id").GetInt64();
        var entity = await _context.StockTransactions.FindAsync(id);
        if (entity == null)
        {
            entity = new StockTransaction { StockTxId = id };
            _context.StockTransactions.Add(entity);
        }
        entity.ProductId = item.GetProperty("product_id").GetInt64();
        entity.TxType = item.TryGetProperty("tx_type", out var tt) ? tt.GetString() ?? "" : "";
        entity.Qty = item.TryGetProperty("qty", out var q) ? q.GetDecimal() : 0;
        entity.UnitCost = item.TryGetProperty("unit_cost", out var uc) && uc.ValueKind != JsonValueKind.Null ? uc.GetDecimal() : null;
        entity.Reference = item.TryGetProperty("reference", out var r) ? r.GetString() : null;
        entity.CreatedByUserId = item.TryGetProperty("created_by_user_id", out var cb) && cb.ValueKind != JsonValueKind.Null ? cb.GetInt64() : null;
        entity.CreatedAt = item.TryGetProperty("created_at", out var ca) ? ca.GetDateTime() : DateTime.Now;
        SetSyncProperties(entity, item);
    }

    private void SetSyncProperties(ISyncable entity, JsonElement item)
    {
        if (item.TryGetProperty("sync_id", out var sid) && sid.ValueKind != JsonValueKind.Null)
        {
            if (Guid.TryParse(sid.GetString(), out var guid))
                entity.SyncId = guid;
        }
        entity.LastModifiedAt = item.TryGetProperty("last_modified_at", out var lm) && lm.ValueKind != JsonValueKind.Null ? lm.GetDateTime() : DateTime.Now;
        entity.LastSyncedAt = DateTime.Now;
        entity.SyncStatus = "synced";
        entity.SyncVersion = item.TryGetProperty("sync_version", out var sv) ? sv.GetInt32() : 1;
    }

    private async Task<SyncResult> PerformSyncAsync(bool syncAll, IProgress<SyncProgress>? progress)
    {
        var result = new SyncResult { StartTime = DateTime.Now };

        if (string.IsNullOrEmpty(_settings.ApiUrl))
        {
            result.Success = false;
            result.ErrorMessage = "Sync is not configured. Please set the API URL in settings.";
            return result;
        }

        try
        {
            _httpClient.DefaultRequestHeaders.Clear();
            if (!string.IsNullOrEmpty(_settings.ApiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("X-API-Key", _settings.ApiKey);
            }

            var entities = new[] { 
                "Role", "Person", "Employee", "UserAccount", "Customer", "ServiceCategory", "Service", 
                "Product", "Inventory", "Supplier", "LedgerAccount", "Appointment", "AppointmentService", 
                "Sale", "SaleItem", "Payment", "Expense", "JournalEntry", "JournalEntryLine", "Payroll",
                "PurchaseOrder", "PurchaseOrderItem", "StockTransaction"
            };
            
            var totalSteps = entities.Length * 2; // Upload + Download for each
            var currentStep = 0;

            foreach (var entityName in entities)
            {
                // Upload pending changes
                progress?.Report(new SyncProgress
                {
                    CurrentEntity = entityName,
                    CurrentOperation = "Uploading",
                    PercentComplete = (int)((double)currentStep / totalSteps * 100)
                });

                var uploadResult = await UploadPendingAsync(entityName);
                result.RecordsUploaded += uploadResult.uploaded;
                result.Errors.AddRange(uploadResult.errors);
                currentStep++;

                // Download new/updated records from server
                progress?.Report(new SyncProgress
                {
                    CurrentEntity = entityName,
                    CurrentOperation = "Downloading",
                    PercentComplete = (int)((double)currentStep / totalSteps * 100)
                });

                var downloadResult = await DownloadUpdatesAsync(entityName, syncAll);
                result.RecordsDownloaded += downloadResult.downloaded;
                result.Errors.AddRange(downloadResult.errors);
                currentStep++;
            }

            result.Success = result.Errors.Count == 0;
            result.EndTime = DateTime.Now;
            _lastSyncTime = result.EndTime;

            progress?.Report(new SyncProgress
            {
                CurrentEntity = "Complete",
                CurrentOperation = "Finished",
                PercentComplete = 100
            });
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = $"Sync failed: {ex.Message}";
            result.EndTime = DateTime.Now;
        }

        return result;
    }

    private async Task<(int uploaded, List<string> errors)> UploadPendingAsync(string entityName)
    {
        var errors = new List<string>();
        var uploaded = 0;

        try
        {
            // Get pending records with projections to avoid navigation property issues
            var (json, pendingIds) = entityName switch
            {
                "Role" => (
                    JsonSerializer.Serialize(await _context.Roles.AsNoTracking()
                        .Select(r => new { r.RoleId, r.Name, r.IsArchived }).ToListAsync()),
                    await _context.Roles.Select(x => (object)x.RoleId).ToListAsync()
                ),
                "Person" => (
                    JsonSerializer.Serialize(await _context.Persons.AsNoTracking().Where(x => x.SyncStatus == "pending")
                        .Select(p => new { p.PersonId, p.SyncId, p.LastModifiedAt, p.LastSyncedAt, p.SyncStatus, p.SyncVersion, p.FirstName, p.LastName, p.Email, p.Phone, p.Address, p.Dob, p.CreatedAt }).ToListAsync()),
                    await _context.Persons.Where(x => x.SyncStatus == "pending").Select(x => (object)x.PersonId).ToListAsync()
                ),
                "Employee" => (
                    JsonSerializer.Serialize(await _context.Employees.AsNoTracking().Where(x => x.SyncStatus == "pending")
                        .Select(e => new { e.EmployeeId, e.SyncId, e.LastModifiedAt, e.LastSyncedAt, e.SyncStatus, e.SyncVersion, e.PersonId, e.RoleId, e.HireDate, e.Status, e.Note, e.CreatedAt }).ToListAsync()),
                    await _context.Employees.Where(x => x.SyncStatus == "pending").Select(x => (object)x.EmployeeId).ToListAsync()
                ),
                "UserAccount" => (
                    JsonSerializer.Serialize(await _context.UserAccounts.AsNoTracking()
                        .Select(u => new { u.UserId, u.EmployeeId, u.Username, u.PasswordHash, u.IsActive, u.LastLogin, u.CreatedAt }).ToListAsync()),
                    await _context.UserAccounts.Select(x => (object)x.UserId).ToListAsync()
                ),
                "Customer" => (
                    JsonSerializer.Serialize(await _context.Customers.AsNoTracking().Where(x => x.SyncStatus == "pending")
                        .Select(c => new { c.CustomerId, c.SyncId, c.LastModifiedAt, c.LastSyncedAt, c.SyncStatus, c.SyncVersion, c.PersonId, c.CustomerCode, c.LoyaltyPoints, c.CreatedAt, c.IsArchived }).ToListAsync()),
                    await _context.Customers.Where(x => x.SyncStatus == "pending").Select(x => (object)x.CustomerId).ToListAsync()
                ),
                "ServiceCategory" => (
                    JsonSerializer.Serialize(await _context.ServiceCategories.AsNoTracking()
                        .Select(s => new { s.ServiceCategoryId, s.Name, s.Description, s.IsArchived }).ToListAsync()),
                    await _context.ServiceCategories.Select(x => (object)x.ServiceCategoryId).ToListAsync()
                ),
                "Service" => (
                    JsonSerializer.Serialize(await _context.Services.AsNoTracking().Where(x => x.SyncStatus == "pending")
                        .Select(s => new { s.ServiceId, s.SyncId, s.LastModifiedAt, s.LastSyncedAt, s.SyncStatus, s.SyncVersion, s.ServiceCategoryId, s.Code, s.Name, s.Description, s.Price, s.DurationMinutes }).ToListAsync()),
                    await _context.Services.Where(x => x.SyncStatus == "pending").Select(x => (object)x.ServiceId).ToListAsync()
                ),
                "Product" => (
                    JsonSerializer.Serialize(await _context.Products.AsNoTracking().Where(x => x.SyncStatus == "pending")
                        .Select(p => new { p.ProductId, p.SyncId, p.LastModifiedAt, p.LastSyncedAt, p.SyncStatus, p.SyncVersion, p.Sku, p.Name, p.Description, p.UnitPrice, p.CostPrice, p.Unit }).ToListAsync()),
                    await _context.Products.Where(x => x.SyncStatus == "pending").Select(x => (object)x.ProductId).ToListAsync()
                ),
                "Inventory" => (
                    JsonSerializer.Serialize(await _context.Inventories.AsNoTracking().Where(x => x.SyncStatus == "pending")
                        .Select(i => new { i.InventoryId, i.SyncId, i.LastModifiedAt, i.LastSyncedAt, i.SyncStatus, i.SyncVersion, i.ProductId, i.QuantityOnHand, i.ReorderLevel, i.LastCountedAt }).ToListAsync()),
                    await _context.Inventories.Where(x => x.SyncStatus == "pending").Select(x => (object)x.InventoryId).ToListAsync()
                ),
                "Supplier" => (
                    JsonSerializer.Serialize(await _context.Suppliers.AsNoTracking().Where(x => x.SyncStatus == "pending")
                        .Select(s => new { s.SupplierId, s.SyncId, s.LastModifiedAt, s.LastSyncedAt, s.SyncStatus, s.SyncVersion, s.Name, s.ContactPerson, s.Phone, s.Email, s.Address, s.IsArchived }).ToListAsync()),
                    await _context.Suppliers.Where(x => x.SyncStatus == "pending").Select(x => (object)x.SupplierId).ToListAsync()
                ),
                "LedgerAccount" => (
                    JsonSerializer.Serialize(await _context.LedgerAccounts.AsNoTracking()
                        .Select(l => new { l.LedgerAccountId, l.Code, l.Name, l.AccountType, l.NormalBalance }).ToListAsync()),
                    await _context.LedgerAccounts.Select(x => (object)x.LedgerAccountId).ToListAsync()
                ),
                "Appointment" => (
                    JsonSerializer.Serialize(await _context.Appointments.AsNoTracking().Where(x => x.SyncStatus == "pending")
                        .Select(a => new { a.AppointmentId, a.SyncId, a.LastModifiedAt, a.LastSyncedAt, a.SyncStatus, a.SyncVersion, a.CustomerId, a.ScheduledStart, a.ScheduledEnd, a.Status, a.Notes, a.CreatedByUserId, a.CreatedAt }).ToListAsync()),
                    await _context.Appointments.Where(x => x.SyncStatus == "pending").Select(x => (object)x.AppointmentId).ToListAsync()
                ),
                "Sale" => (
                    JsonSerializer.Serialize(await _context.Sales.AsNoTracking().Where(x => x.SyncStatus == "pending")
                        .Select(s => new { s.SaleId, s.SyncId, s.LastModifiedAt, s.LastSyncedAt, s.SyncStatus, s.SyncVersion, s.CustomerId, s.CreatedByUserId, s.SaleNumber, s.TotalAmount, s.PaymentStatus, s.CreatedAt }).ToListAsync()),
                    await _context.Sales.Where(x => x.SyncStatus == "pending").Select(x => (object)x.SaleId).ToListAsync()
                ),
                "SaleItem" => (
                    JsonSerializer.Serialize(await _context.SaleItems.AsNoTracking().Where(x => x.SyncStatus == "pending")
                        .Select(s => new { s.SaleItemId, s.SyncId, s.LastModifiedAt, s.LastSyncedAt, s.SyncStatus, s.SyncVersion, s.SaleId, s.ItemType, s.ProductId, s.ServiceId, s.Qty, s.UnitPrice, s.LineTotal, s.TherapistEmployeeId }).ToListAsync()),
                    await _context.SaleItems.Where(x => x.SyncStatus == "pending").Select(x => (object)x.SaleItemId).ToListAsync()
                ),
                "Payment" => (
                    JsonSerializer.Serialize(await _context.Payments.AsNoTracking().Where(x => x.SyncStatus == "pending")
                        .Select(p => new { p.PaymentId, p.SyncId, p.LastModifiedAt, p.LastSyncedAt, p.SyncStatus, p.SyncVersion, p.SaleId, p.PaymentMethod, p.Amount, p.PaidAt, p.RecordedByUserId }).ToListAsync()),
                    await _context.Payments.Where(x => x.SyncStatus == "pending").Select(x => (object)x.PaymentId).ToListAsync()
                ),
                "Expense" => (
                    JsonSerializer.Serialize(await _context.Expenses.AsNoTracking().Where(x => x.SyncStatus == "pending")
                        .Select(e => new { e.ExpenseId, e.SyncId, e.LastModifiedAt, e.LastSyncedAt, e.SyncStatus, e.SyncVersion, e.ExpenseDate, e.Category, e.Description, e.Amount, e.Vendor, e.PaymentMethod, e.CreatedByUserId, e.CreatedAt }).ToListAsync()),
                    await _context.Expenses.Where(x => x.SyncStatus == "pending").Select(x => (object)x.ExpenseId).ToListAsync()
                ),
                "JournalEntry" => (
                    JsonSerializer.Serialize(await _context.JournalEntries.AsNoTracking().Where(x => x.SyncStatus == "pending")
                        .Select(j => new { j.JournalId, j.SyncId, j.LastModifiedAt, j.LastSyncedAt, j.SyncStatus, j.SyncVersion, j.JournalNo, j.Date, j.Description, j.CreatedByUserId, j.CreatedAt }).ToListAsync()),
                    await _context.JournalEntries.Where(x => x.SyncStatus == "pending").Select(x => (object)x.JournalId).ToListAsync()
                ),
                "Payroll" => (
                    JsonSerializer.Serialize(await _context.Payrolls.AsNoTracking().Where(x => x.SyncStatus == "pending")
                        .Select(p => new { p.PayrollId, p.SyncId, p.LastModifiedAt, p.LastSyncedAt, p.SyncStatus, p.SyncVersion, p.EmployeeId, p.PeriodStart, p.PeriodEnd, p.DaysWorked, p.DailyRate, p.GrossPay, p.Deductions, p.NetPay, p.Status, p.PaidAt, p.CreatedByUserId, p.CreatedAt }).ToListAsync()),
                    await _context.Payrolls.Where(x => x.SyncStatus == "pending").Select(x => (object)x.PayrollId).ToListAsync()
                ),
                "PurchaseOrder" => (
                    JsonSerializer.Serialize(await _context.PurchaseOrders.AsNoTracking().Where(x => x.SyncStatus == "pending")
                        .Select(p => new { p.PoId, p.SyncId, p.LastModifiedAt, p.LastSyncedAt, p.SyncStatus, p.SyncVersion, p.PoNumber, p.SupplierId, p.Status, p.CreatedByUserId, p.CreatedAt }).ToListAsync()),
                    await _context.PurchaseOrders.Where(x => x.SyncStatus == "pending").Select(x => (object)x.PoId).ToListAsync()
                ),
                "PurchaseOrderItem" => (
                    JsonSerializer.Serialize(await _context.PurchaseOrderItems.AsNoTracking().Where(x => x.SyncStatus == "pending")
                        .Select(p => new { p.PoItemId, p.SyncId, p.LastModifiedAt, p.LastSyncedAt, p.SyncStatus, p.SyncVersion, p.PoId, p.ProductId, p.QtyOrdered, p.UnitCost }).ToListAsync()),
                    await _context.PurchaseOrderItems.Where(x => x.SyncStatus == "pending").Select(x => (object)x.PoItemId).ToListAsync()
                ),
                "StockTransaction" => (
                    JsonSerializer.Serialize(await _context.StockTransactions.AsNoTracking().Where(x => x.SyncStatus == "pending")
                        .Select(s => new { s.StockTxId, s.SyncId, s.LastModifiedAt, s.LastSyncedAt, s.SyncStatus, s.SyncVersion, s.ProductId, s.TxType, s.Qty, s.UnitCost, s.Reference, s.CreatedByUserId, s.CreatedAt }).ToListAsync()),
                    await _context.StockTransactions.Where(x => x.SyncStatus == "pending").Select(x => (object)x.StockTxId).ToListAsync()
                ),
                "JournalEntryLine" => (
                    JsonSerializer.Serialize(await _context.JournalEntryLines.AsNoTracking().Where(x => x.SyncStatus == "pending")
                        .Select(j => new { j.JournalLineId, j.SyncId, j.LastModifiedAt, j.LastSyncedAt, j.SyncStatus, j.SyncVersion, j.JournalId, j.LedgerAccountId, j.Debit, j.Credit, j.LineMemo }).ToListAsync()),
                    await _context.JournalEntryLines.Where(x => x.SyncStatus == "pending").Select(x => (object)x.JournalLineId).ToListAsync()
                ),
                "AppointmentService" => (
                    JsonSerializer.Serialize(await _context.AppointmentServices.AsNoTracking().Where(x => x.SyncStatus == "pending")
                        .Select(a => new { a.ApptSrvId, a.SyncId, a.LastModifiedAt, a.LastSyncedAt, a.SyncStatus, a.SyncVersion, a.AppointmentId, a.ServiceId, a.TherapistEmployeeId, a.Price, a.CommissionAmount }).ToListAsync()),
                    await _context.AppointmentServices.Where(x => x.SyncStatus == "pending").Select(x => (object)x.ApptSrvId).ToListAsync()
                ),
                _ => ("[]", new List<object>())
            };

            if (pendingIds.Count == 0)
                return (0, errors);

            // Send to server
            var endpointName = GetEndpointName(entityName);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            Console.WriteLine($"[Sync] Uploading {pendingIds.Count} {entityName} records to {_settings.ApiUrl}/api/data/upload/{endpointName}");
            
            var response = await _httpClient.PostAsync(
                $"{_settings.ApiUrl.TrimEnd('/')}/api/data/upload/{endpointName}",
                content);

            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Sync] Response for {entityName}: {response.StatusCode} - {responseBody}");

            if (response.IsSuccessStatusCode)
            {
                // Parse response to verify records were actually processed
                try
                {
                    var responseJson = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    if (responseJson.TryGetProperty("success", out var successProp) && successProp.GetBoolean())
                    {
                        var recordsProcessed = responseJson.TryGetProperty("recordsProcessed", out var countProp) 
                            ? countProp.GetInt32() 
                            : pendingIds.Count;
                        
                        // Check for partial failures
                        if (responseJson.TryGetProperty("errors", out var errorsProp) && 
                            errorsProp.ValueKind == JsonValueKind.Array && 
                            errorsProp.GetArrayLength() > 0)
                        {
                            foreach (var err in errorsProp.EnumerateArray())
                            {
                                errors.Add($"Server error for {entityName}: {err.GetString()}");
                            }
                        }
                        
                        // Only mark as synced if records were actually processed
                        if (recordsProcessed > 0)
                        {
                            await MarkAsSyncedAsync(entityName, pendingIds);
                            uploaded = recordsProcessed;
                            Console.WriteLine($"[Sync] Successfully synced {recordsProcessed} {entityName} records");
                        }
                    }
                    else
                    {
                        var errorMsg = responseJson.TryGetProperty("error", out var errProp) 
                            ? errProp.GetString() 
                            : "Unknown server error";
                        errors.Add($"Server rejected {entityName}: {errorMsg}");
                    }
                }
                catch (JsonException)
                {
                    // Response wasn't valid JSON, but status was OK - assume success
                    await MarkAsSyncedAsync(entityName, pendingIds);
                    uploaded = pendingIds.Count;
                }
            }
            else
            {
                errors.Add($"Failed to upload {entityName}: HTTP {(int)response.StatusCode} {response.StatusCode} - {responseBody}");
            }
        }
        catch (HttpRequestException ex)
        {
            // Server not available - report the error with details
            errors.Add($"Connection error for {entityName}: {ex.Message}");
            Console.WriteLine($"[Sync] Connection error for {entityName}: {ex}");
        }
        catch (TaskCanceledException ex)
        {
            errors.Add($"Timeout uploading {entityName}: Request timed out after 30 seconds");
            Console.WriteLine($"[Sync] Timeout for {entityName}: {ex.Message}");
        }
        catch (Exception ex)
        {
            errors.Add($"Error uploading {entityName}: {ex.Message}");
            Console.WriteLine($"[Sync] Error uploading {entityName}: {ex}");
        }

        return (uploaded, errors);
    }

    private string GetEndpointName(string entityName)
    {
        return entityName switch
        {
            "Role" => "roles",
            "Person" => "persons",
            "Employee" => "employees",
            "UserAccount" => "useraccounts",
            "Customer" => "customers",
            "ServiceCategory" => "servicecategories",
            "Service" => "services",
            "Product" => "products",
            "Inventory" => "inventories",
            "Supplier" => "suppliers",
            "LedgerAccount" => "ledgeraccounts",
            "Appointment" => "appointments",
            "AppointmentService" => "appointmentservices",
            "Sale" => "sales",
            "SaleItem" => "saleitems",
            "Payment" => "payments",
            "Expense" => "expenses",
            "JournalEntry" => "journalentries",
            "JournalEntryLine" => "journalentrylines",
            "Payroll" => "payrolls",
            "PurchaseOrder" => "purchaseorders",
            "PurchaseOrderItem" => "purchaseorderitems",
            "StockTransaction" => "stocktransactions",
            _ => entityName.ToLower() + "s"
        };
    }

    private async Task MarkAsSyncedAsync(string entityName, List<object> ids)
    {
        if (ids.Count == 0) return;

        foreach (var id in ids)
        {
            var entity = entityName switch
            {
                "Person" => await _context.Persons.FindAsync(Convert.ToInt64(id)) as ISyncable,
                "Customer" => await _context.Customers.FindAsync(Convert.ToInt64(id)) as ISyncable,
                "Employee" => await _context.Employees.FindAsync(Convert.ToInt64(id)) as ISyncable,
                "Service" => await _context.Services.FindAsync(Convert.ToInt64(id)) as ISyncable,
                "Product" => await _context.Products.FindAsync(Convert.ToInt64(id)) as ISyncable,
                "Appointment" => await _context.Appointments.FindAsync(Convert.ToInt64(id)) as ISyncable,
                "AppointmentService" => await _context.AppointmentServices.FindAsync(Convert.ToInt64(id)) as ISyncable,
                "Sale" => await _context.Sales.FindAsync(Convert.ToInt64(id)) as ISyncable,
                "SaleItem" => await _context.SaleItems.FindAsync(Convert.ToInt64(id)) as ISyncable,
                "Payment" => await _context.Payments.FindAsync(Convert.ToInt64(id)) as ISyncable,
                "Expense" => await _context.Expenses.FindAsync(Convert.ToInt64(id)) as ISyncable,
                "Inventory" => await _context.Inventories.FindAsync(Convert.ToInt64(id)) as ISyncable,
                "Payroll" => await _context.Payrolls.FindAsync(Convert.ToInt64(id)) as ISyncable,
                "JournalEntry" => await _context.JournalEntries.FindAsync(Convert.ToInt64(id)) as ISyncable,
                "JournalEntryLine" => await _context.JournalEntryLines.FindAsync(Convert.ToInt64(id)) as ISyncable,
                "PurchaseOrder" => await _context.PurchaseOrders.FindAsync(Convert.ToInt64(id)) as ISyncable,
                "PurchaseOrderItem" => await _context.PurchaseOrderItems.FindAsync(Convert.ToInt64(id)) as ISyncable,
                "Supplier" => await _context.Suppliers.FindAsync(Convert.ToInt64(id)) as ISyncable,
                "StockTransaction" => await _context.StockTransactions.FindAsync(Convert.ToInt64(id)) as ISyncable,
                _ => null
            };

            if (entity != null)
            {
                entity.SyncStatus = "synced";
                entity.LastSyncedAt = DateTime.Now;
            }
        }
        await _context.SaveChangesAsync();
    }

    private async Task<(int downloaded, List<string> errors)> DownloadUpdatesAsync(string entityName, bool syncAll)
    {
        var errors = new List<string>();
        var downloaded = 0;

        try
        {
            var response = await _httpClient.GetAsync(
                $"{_settings.ApiUrl.TrimEnd('/')}/api/data/download/{entityName.ToLower()}s");

            if (response.IsSuccessStatusCode)
            {
                // Process downloaded data based on entity type
                // This would merge/update local records
                // For now, just count the response
                var content = await response.Content.ReadAsStringAsync();
                // TODO: Implement actual merge logic when API is ready
                downloaded = 0; // Placeholder
            }
        }
        catch (HttpRequestException)
        {
            // Server not available - that's okay
        }
        catch (Exception ex)
        {
            errors.Add($"Error downloading {entityName}: {ex.Message}");
        }

        return (downloaded, errors);
    }
}

// Supporting classes

public class SyncSettings
{
    public string ApiUrl { get; set; } = "";
    public string ApiKey { get; set; } = "";
    public bool AutoSyncEnabled { get; set; } = false;
    public int SyncIntervalMinutes { get; set; } = 60; // Default: every hour
    public string DeviceName { get; set; } = Environment.MachineName;
}

public class SyncStatusSummary
{
    public DateTime? LastSyncTime { get; set; }
    public bool IsConfigured { get; set; }
    public bool AutoSyncEnabled { get; set; }
    public int SyncIntervalMinutes { get; set; }
    public int TotalPending { get; set; }
    public Dictionary<string, int> PendingCounts { get; set; } = new();
}

public class SyncResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int RecordsUploaded { get; set; }
    public int RecordsDownloaded { get; set; }
    public int Conflicts { get; set; }
    public List<string> Errors { get; set; } = new();

    public TimeSpan Duration => (EndTime ?? DateTime.Now) - StartTime;
}

public class SyncProgress
{
    public string CurrentEntity { get; set; } = "";
    public string CurrentOperation { get; set; } = "";
    public int PercentComplete { get; set; }
}
