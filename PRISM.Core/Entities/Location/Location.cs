namespace PRISM.Core.Entities.Location;

using PRISM.Core.Entities.Contract;

/// <summary>
/// Represents a physical location for deliveries
/// </summary>
public class Location : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public decimal? ShippingRate { get; set; }

    // Navigation property
    public virtual ICollection<ContractDelivery> Deliveries { get; set; } = new List<ContractDelivery>();
}