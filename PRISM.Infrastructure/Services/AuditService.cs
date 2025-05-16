namespace PRISM.Infrastructure.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PRISM.Core.Entities.Audit;
using PRISM.Core.Interfaces;
using PRISM.Data;

/// <summary>
/// Implementation of the audit logging service
/// </summary>
public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuditService> _logger;

    public AuditService(
        ApplicationDbContext dbContext, 
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuditService> logger)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task LogAuditAsync(string userId, string entityName, int entityId, AuditAction action, string details = "")
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var ipAddress = httpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";
            var userAgent = httpContext?.Request?.Headers["User-Agent"].ToString() ?? "Unknown";

            var auditLog = new AuditLog
            {
                UserId = userId,
                EntityName = entityName,
                EntityId = entityId,
                Action = action,
                Details = details,
                Timestamp = DateTime.UtcNow,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            _dbContext.Set<AuditLog>().Add(auditLog);
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // Handle the case where the AuditLog table doesn't exist yet
            _logger.LogWarning(ex, "Unable to log audit entry. The AuditLog table may not exist yet. " +
                "EntityName: {EntityName}, EntityId: {EntityId}, Action: {Action}", 
                entityName, entityId, action);
        }
        catch (Exception ex)
        {
            // Log the error but don't throw - audit logging should not block the main operation
            _logger.LogError(ex, "Error occurred while logging audit entry. " +
                "EntityName: {EntityName}, EntityId: {EntityId}, Action: {Action}",
                entityName, entityId, action);
        }
    }
}