namespace PRISM.Core.Entities.Supplier;

/// <summary>
/// Represents a contact person for a supplier
/// </summary>
public class SupplierContact : BaseEntity
{
    public int SupplierId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsPrimary { get; set; } = false;
    public bool IsActive { get; set; } = true;

    // Navigation property
    public virtual Supplier Supplier { get; set; } = null!;
}