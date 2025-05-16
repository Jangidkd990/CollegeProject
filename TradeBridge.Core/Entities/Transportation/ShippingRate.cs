namespace TradeBridge.Core.Entities.Transportation;

using TradeBridge.Core.Entities.Location;
using System;

/// <summary>
/// Represents a shipping rate between locations
/// </summary>
public class ShippingRate : BaseEntity
{
    /// <summary>
    /// ID of the origin location
    /// </summary>
    public int OriginLocationId { get; set; }

    /// <summary>
    /// Navigation property for origin location
    /// </summary>
    public Location OriginLocation { get; set; } = null!;

    /// <summary>
    /// ID of the destination location
    /// </summary>
    public int DestinationLocationId { get; set; }

    /// <summary>
    /// Navigation property for destination location
    /// </summary>
    public Location DestinationLocation { get; set; } = null!;

    /// <summary>
    /// Transportation mode (e.g., Truck, Rail, Ship)
    /// </summary>
    public string TransportMode { get; set; } = null!;

    /// <summary>
    /// Effective start date for this rate
    /// </summary>
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// Expiration date for this rate (null means no expiration)
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// Base rate per unit
    /// </summary>
    public decimal BaseRate { get; set; }

    /// <summary>
    /// Fuel surcharge percentage
    /// </summary>
    public decimal FuelSurchargePercent { get; set; }

    /// <summary>
    /// Additional fees
    /// </summary>
    public decimal AdditionalFees { get; set; }

    /// <summary>
    /// Minimum quantity for this rate to apply
    /// </summary>
    public decimal? MinimumQuantity { get; set; }

    /// <summary>
    /// Name of the carrier or vendor
    /// </summary>
    public string Carrier { get; set; } = null!;

    /// <summary>
    /// Contract or quote reference number
    /// </summary>
    public string? ReferenceNumber { get; set; }

    /// <summary>
    /// Notes about this shipping rate
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Calculates the total rate including surcharges and fees
    /// </summary>
    /// <returns>The total shipping rate per unit</returns>
    public decimal CalculateTotalRate()
    {
        return BaseRate + (BaseRate * FuelSurchargePercent / 100) + AdditionalFees;
    }
}