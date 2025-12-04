using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Spa_Sync_API.Data;
using System.Text.Json;

namespace Spa_Sync_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataController : ControllerBase
{
    private readonly CloudDbContext _context;
    private readonly ILogger<DataController> _logger;
    private readonly IConfiguration _configuration;

    public DataController(CloudDbContext context, ILogger<DataController> logger, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet("status")]
    public IActionResult Status()
    {
        return Ok(new {
            status = "online",
            version = "2.6.0",  // Updated version with expanded sync support
            timestamp = DateTime.UtcNow,
            database = _context.Database.CanConnect() ? "connected" : "disconnected"
        });
    }

    /// <summary>
    /// Get row counts for all tables - useful for verifying sync
    /// </summary>
    [HttpGet("counts")]
    public async Task<IActionResult> GetCounts()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var counts = new Dictionary<string, int>();
            
            var tables = new[] 
            { 
                "Person", "Employee", "Customer", "Role", "UserAccount",
                "ServiceCategory", "Service", "Product", "Inventory", "Supplier", "SupplierProduct",
                "Appointment", "AppointmentService", "Sale", "SaleItem", "Payment",
                "Expense", "Payroll", "JournalEntry", "JournalEntryLine", "LedgerAccount",
                "PurchaseOrder", "PurchaseOrderItem", "StockTransaction",
                "EmployeeAttendance", "EmployeeServiceCommission", "AuditLog", "CRM_Note"
            };

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            foreach (var table in tables)
            {
                try
                {
                    using var cmd = new SqlCommand($"SELECT COUNT(*) FROM [{table}]", connection);
                    var count = (int)await cmd.ExecuteScalarAsync();
                    counts[table] = count;
                }
                catch
                {
                    counts[table] = -1; // Table doesn't exist or error
                }
            }

            return Ok(new { success = true, counts, timestamp = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting counts");
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Verify specific records exist in the cloud
    /// </summary>
    [HttpPost("verify/{entityType}")]
    public async Task<IActionResult> VerifyRecords(string entityType, [FromBody] JsonElement ids)
    {
        try
        {
            var tableName = GetTableName(entityType.ToLower());
            if (tableName == null)
            {
                return BadRequest(new { success = false, error = $"Unknown entity type: {entityType}" });
            }

            var pkColumn = GetPrimaryKeyColumn(tableName);
            var idList = ids.EnumerateArray().Select(x => x.GetInt64()).ToList();
            
            if (idList.Count == 0)
            {
                return Ok(new { success = true, found = new List<long>(), missing = new List<long>() });
            }

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var sql = $"SELECT [{pkColumn}] FROM [{tableName}] WHERE [{pkColumn}] IN ({string.Join(",", idList)})";
            using var cmd = new SqlCommand(sql, connection);
            using var reader = await cmd.ExecuteReaderAsync();
            
            var found = new List<long>();
            while (await reader.ReadAsync())
            {
                found.Add(reader.GetInt64(0));
            }

            var missing = idList.Except(found).ToList();

            return Ok(new { 
                success = true, 
                found = found, 
                missing = missing,
                allPresent = missing.Count == 0
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying records for {EntityType}", entityType);
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    [HttpPost("upload/{entityType}")]
    public async Task<IActionResult> Upload(string entityType, [FromBody] JsonElement data)
    {
        try
        {
            var tableName = GetTableName(entityType.ToLower());
            if (tableName == null)
            {
                return BadRequest(new { success = false, error = $"Unknown entity type: {entityType}" });
            }

            var items = data.EnumerateArray().ToList();
            if (items.Count == 0)
            {
                return Ok(new { success = true, recordsProcessed = 0 });
            }

            int processed = 0;
            var errors = new List<string>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            foreach (var item in items)
            {
                try
                {
                    await UpsertWithIdentityInsert(tableName, item, connectionString);
                    processed++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error upserting item to {Table}", tableName);
                    errors.Add(ex.Message);
                }
            }

            return Ok(new { success = true, recordsProcessed = processed, errors = errors.Count > 0 ? errors : null });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading {EntityType}", entityType);
            return BadRequest(new {
                success = false,
                error = ex.Message,
                details = ex.InnerException?.Message ?? ex.ToString()
            });
        }
    }

    private async Task UpsertWithIdentityInsert(string tableName, JsonElement item, string connectionString)
    {
        var pkColumn = GetPrimaryKeyColumn(tableName);
        var hasSyncColumns = HasSyncColumns(tableName);

        var columns = new List<string>();
        var values = new List<string>();
        var updatePairs = new List<string>();
        var parameters = new Dictionary<string, object?>();
        int paramIndex = 0;

        foreach (var prop in item.EnumerateObject())
        {
            var columnName = ToSnakeCase(prop.Name);
            var paramName = $"@p{paramIndex++}";

            // Skip sync_status and last_synced_at from client - we'll set them server-side
            if (hasSyncColumns && (columnName == "sync_status" || columnName == "last_synced_at"))
            {
                continue;
            }

            columns.Add($"[{columnName}]");
            values.Add(paramName);

            if (!columnName.Equals(pkColumn, StringComparison.OrdinalIgnoreCase))
            {
                updatePairs.Add($"[{columnName}] = {paramName}");
            }

            parameters[paramName] = GetParameterValue(prop.Value);
        }

        // For tables with sync columns, add server-side sync metadata
        if (hasSyncColumns)
        {
            var syncedAtParam = $"@p{paramIndex++}";
            var syncStatusParam = $"@p{paramIndex++}";
            
            columns.Add("[sync_status]");
            values.Add(syncStatusParam);
            updatePairs.Add($"[sync_status] = {syncStatusParam}");
            parameters[syncStatusParam] = "synced";
            
            columns.Add("[last_synced_at]");
            values.Add(syncedAtParam);
            updatePairs.Add($"[last_synced_at] = {syncedAtParam}");
            parameters[syncedAtParam] = DateTime.UtcNow;
        }

        if (updatePairs.Count == 0)
        {
            updatePairs.Add($"[{pkColumn}] = [{pkColumn}]");
        }

        var sql = $@"
            SET IDENTITY_INSERT [{tableName}] ON;

            MERGE [{tableName}] AS target
            USING (SELECT {string.Join(", ", columns.Select((c, i) => $"{values[i]} AS {c}"))}) AS source
            ON target.[{pkColumn}] = source.[{pkColumn}]
            WHEN MATCHED THEN
                UPDATE SET {string.Join(", ", updatePairs)}
            WHEN NOT MATCHED THEN
                INSERT ({string.Join(", ", columns)})
                VALUES ({string.Join(", ", values)});

            SET IDENTITY_INSERT [{tableName}] OFF;
        ";

        // Use a fresh connection for each operation
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(sql, connection);
        foreach (var param in parameters)
        {
            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
        }

        await command.ExecuteNonQueryAsync();
    }

    private string ToSnakeCase(string camelCase)
    {
        if (string.IsNullOrEmpty(camelCase)) return camelCase;

        var result = new System.Text.StringBuilder();
        result.Append(char.ToLower(camelCase[0]));

        for (int i = 1; i < camelCase.Length; i++)
        {
            if (char.IsUpper(camelCase[i]))
            {
                result.Append('_');
                result.Append(char.ToLower(camelCase[i]));
            }
            else
            {
                result.Append(camelCase[i]);
            }
        }

        return result.ToString();
    }

    private object? GetParameterValue(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.TryGetInt64(out var l) ? l : element.GetDecimal(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            JsonValueKind.Undefined => null,
            _ => element.GetRawText()
        };
    }

    private bool HasSyncColumns(string tableName)
    {
        // Tables that have sync_id, sync_status, last_synced_at, etc.
        var syncableTables = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Person", "Employee", "Customer", "Service", "Product", "Inventory",
            "Appointment", "AppointmentService", "Sale", "SaleItem", "Payment", "Expense", 
            "JournalEntry", "JournalEntryLine", "Payroll", "PurchaseOrder", "PurchaseOrderItem",
            "Supplier", "StockTransaction"
        };
        return syncableTables.Contains(tableName);
    }

    private string GetPrimaryKeyColumn(string tableName)
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
            "SupplierProduct" => "supplier_product_id",
            "PurchaseOrder" => "po_id",
            "PurchaseOrderItem" => "po_item_id",
            "StockTransaction" => "stock_tx_id",
            "Appointment" => "appointment_id",
            "AppointmentService" => "appt_srv_id",
            "Sale" => "sale_id",
            "SaleItem" => "sale_item_id",
            "Payment" => "payment_id",
            "LedgerAccount" => "ledger_account_id",
            "JournalEntry" => "journal_id",
            "JournalEntryLine" => "journal_line_id",
            "EmployeeServiceCommission" => "commission_id",
            "Expense" => "expense_id",
            "Payroll" => "payroll_id",
            "EmployeeAttendance" => "attendance_id",
            "CrmNote" => "note_id",
            "CRM_Note" => "note_id",
            "AuditLog" => "audit_id",
            _ => throw new Exception($"Unknown table: {tableName}")
        };
    }

    private string? GetTableName(string entityType)
    {
        return entityType switch
        {
            "roles" => "Role",
            "persons" => "Person",
            "employees" => "Employee",
            "customers" => "Customer",
            "useraccounts" => "UserAccount",
            "servicecategories" => "ServiceCategory",
            "services" => "Service",
            "products" => "Product",
            "inventories" => "Inventory",
            "suppliers" => "Supplier",
            "purchaseorders" => "PurchaseOrder",
            "purchaseorderitems" => "PurchaseOrderItem",
            "stocktransactions" => "StockTransaction",
            "supplierproducts" => "SupplierProduct",
            "appointments" => "Appointment",
            "appointmentservices" => "AppointmentService",
            "sales" => "Sale",
            "saleitems" => "SaleItem",
            "payments" => "Payment",
            "ledgeraccounts" => "LedgerAccount",
            "journalentries" => "JournalEntry",
            "journalentrylines" => "JournalEntryLine",
            "employeeservicecommissions" => "EmployeeServiceCommission",
            "expenses" => "Expense",
            "payrolls" => "Payroll",
            "employeeattendances" => "EmployeeAttendance",
            "crmnotes" => "CRM_Note",
            "auditlogs" => "AuditLog",
            _ => null
        };
    }

    [HttpGet("download/{entityType}")]
    public async Task<IActionResult> Download(string entityType)
    {
        try
        {
            var tableName = GetTableName(entityType.ToLower());
            if (tableName == null)
            {
                return BadRequest(new { success = false, error = $"Unknown entity type: {entityType}" });
            }

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var sql = $"SELECT * FROM [{tableName}]";

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            var results = new List<Dictionary<string, object?>>();

            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object?>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    row[reader.GetName(i)] = value;
                }
                results.Add(row);
            }

            return Ok(new { success = true, data = results, count = results.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading {EntityType}", entityType);
            return BadRequest(new {
                success = false,
                error = ex.Message,
                details = ex.InnerException?.Message ?? ex.ToString()
            });
        }
    }

    /// <summary>
    /// Reset/clear all data from cloud database for fresh sync
    /// </summary>
    [HttpPost("reset")]
    public async Task<IActionResult> ResetDatabase()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            // Delete in reverse dependency order (children first, then parents)
            var deleteOrder = new[]
            {
                "AuditLog", "CrmNote", "EmployeeAttendance", "EmployeeServiceCommission",
                "JournalEntryLine", "JournalEntry", "LedgerAccount",
                "Payment", "SaleItem", "Sale",
                "AppointmentService", "Appointment",
                "StockTransaction", "PurchaseOrderItem", "PurchaseOrder",
                "Inventory", "Product", "Supplier",
                "Payroll", "Expense",
                "Service", "ServiceCategory",
                "UserAccount", "Customer", "Employee", "Person", "Role"
            };

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var deleted = new Dictionary<string, int>();

            foreach (var table in deleteOrder)
            {
                try
                {
                    using var cmd = new SqlCommand($"DELETE FROM [{table}]", connection);
                    var count = await cmd.ExecuteNonQueryAsync();
                    deleted[table] = count;
                    _logger.LogInformation("Deleted {Count} rows from {Table}", count, table);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Could not delete from {Table}: {Error}", table, ex.Message);
                    deleted[table] = -1;
                }
            }

            return Ok(new { 
                success = true, 
                message = "Cloud database has been reset",
                deleted = deleted,
                timestamp = DateTime.UtcNow 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting database");
            return BadRequest(new { success = false, error = ex.Message });
        }
    }
}
