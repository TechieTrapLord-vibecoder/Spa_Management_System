using Spa_Management_System.Data;
using Spa_Management_System.Models;

namespace Spa_Management_System.Services;

public interface IAuditLogService
{
    Task LogCreateAsync(string entityName, string entityId, string? summary, long? userId);
    Task LogUpdateAsync(string entityName, string entityId, string? summary, long? userId);
    Task LogDeleteAsync(string entityName, string entityId, string? summary, long? userId);
    Task LogAsync(string entityName, string entityId, string action, string? summary, long? userId);
    Task<List<AuditLog>> GetLogsAsync(string? entityName = null, int? days = null, int maxRecords = 100);
}

public class AuditLogService : IAuditLogService
{
    private readonly AppDbContext _context;

    public AuditLogService(AppDbContext context)
    {
        _context = context;
    }

    public async Task LogCreateAsync(string entityName, string entityId, string? summary, long? userId)
    {
        await LogAsync(entityName, entityId, "create", summary, userId);
    }

    public async Task LogUpdateAsync(string entityName, string entityId, string? summary, long? userId)
    {
        await LogAsync(entityName, entityId, "update", summary, userId);
    }

    public async Task LogDeleteAsync(string entityName, string entityId, string? summary, long? userId)
    {
        await LogAsync(entityName, entityId, "delete", summary, userId);
    }

    public async Task LogAsync(string entityName, string entityId, string action, string? summary, long? userId)
    {
        try
        {
            var auditLog = new AuditLog
            {
                EntityName = entityName,
                EntityId = entityId,
                Action = action,
                ChangeSummary = summary,
                ChangedByUserId = userId,
                CreatedAt = DateTime.Now
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }
        catch
        {
            // Don't let audit logging failures crash the app
            // In production, you'd want to log this to a file or monitoring service
        }
    }

    public async Task<List<AuditLog>> GetLogsAsync(string? entityName = null, int? days = null, int maxRecords = 100)
    {
        var query = _context.AuditLogs.AsQueryable();

        if (!string.IsNullOrEmpty(entityName))
        {
            query = query.Where(a => a.EntityName == entityName);
        }

        if (days.HasValue)
        {
            var cutoff = DateTime.Now.AddDays(-days.Value);
            query = query.Where(a => a.CreatedAt >= cutoff);
        }

        return await Task.FromResult(
            query.OrderByDescending(a => a.CreatedAt)
                 .Take(maxRecords)
                 .ToList()
        );
    }
}
