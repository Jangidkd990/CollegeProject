namespace PRISM.Core.Interfaces;

/// <summary>
/// Interface for audit logging service
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Logs an audit event for CRUD operations
    /// </summary>
    /// <param name="userId">The ID of the user who performed the action</param>
    /// <param name="entityName">The name of the entity that was modified</param>
    /// <param name="entityId">The ID of the entity that was modified</param>
    /// <param name="action">The type of action performed</param>
    /// <param name="details">Any additional details about the action</param>
    Task LogAuditAsync(string userId, string entityName, int entityId, AuditAction action, string details = "");
}

/// <summary>
/// Types of actions that can be audited
/// </summary>
public enum AuditAction
{
    Create,
    Read,
    Update,
    Delete,
    Login,
    Logout,
    Export
}