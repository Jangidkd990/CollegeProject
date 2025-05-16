namespace PRISM.Core.Entities.Contract;

using PRISM.Core.Entities.Location;

/// <summary>
/// Represents a delivery schedule/forecast for a contract
/// </summary>
public class ContractDelivery : BaseEntity
{
    public int ContractId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public decimal Quantity { get; set; }
    public decimal? ForecastedPrice { get; set; }
    public decimal? ActualPrice { get; set; }
    public bool IsDeferred { get; set; } = false;
    public DateTime? DeferredToDate { get; set; }
    public int? LocationId { get; set; }
    public string Notes { get; set; } = string.Empty;

    // Navigation properties
    public virtual Contract Contract { get; set; } = null!;
    public virtual Location DeliveryLocation { get; set; } = null!;
}