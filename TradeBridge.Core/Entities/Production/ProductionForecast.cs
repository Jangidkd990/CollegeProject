namespace TradeBridge.Core.Entities.Production;

using TradeBridge.Core.Entities.Location;

/// <summary>
/// Represents a production forecast for planning purposes
/// </summary>
public class ProductionForecast : BaseEntity
{
    /// <summary>
    /// Date when the forecast was created
    /// </summary>
    public DateTime ForecastDate { get; set; }
    
    /// <summary>
    /// Date when the production is scheduled
    /// </summary>
    public DateTime ProductionDate { get; set; }
    
    /// <summary>
    /// ID of the location where production will take place
    /// </summary>
    public int LocationId { get; set; }
    
    /// <summary>
    /// Navigation property for production location
    /// </summary>
    public Location Location { get; set; } = null!;
    
    /// <summary>
    /// Planned production quantity
    /// </summary>
    public decimal PlannedQuantity { get; set; }
    
    /// <summary>
    /// Actual production quantity (if available)
    /// </summary>
    public decimal? ActualQuantity { get; set; }
    
    /// <summary>
    /// Variance percentage between planned and actual production quantities
    /// </summary>
    public decimal? VariancePercent { get; set; }
    
    /// <summary>
    /// Additional notes about the production forecast
    /// </summary>
    public string Notes { get; set; } = string.Empty;
    
    /// <summary>
    /// Indicates if this is a quarterly forecast
    /// </summary>
    public bool IsQuarterly { get; set; } = false;
    
    /// <summary>
    /// Quarter number (1-4) if this is a quarterly forecast
    /// </summary>
    public int? QuarterNumber { get; set; }
    
    /// <summary>
    /// Year of the forecast
    /// </summary>
    public int? Year { get; set; }
    
    /// <summary>
    /// Indicates if this forecast is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}