namespace TradeBridge.Core.Entities.Transportation;

using TradeBridge.Core.Entities.Contract;
using TradeBridge.Core.Entities.Location;
using System;
using System.Collections.Generic;

/// <summary>
/// Represents a transportation plan for contract deliveries
/// </summary>
public class TransportationPlan : BaseEntity
{
    /// <summary>
    /// Reference number for the transportation plan
    /// </summary>
    public string PlanNumber { get; set; } = null!;

    /// <summary>
    /// Start date of the transportation plan
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// End date of the transportation plan
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Origin location ID
    /// </summary>
    public int OriginLocationId { get; set; }

    /// <summary>
    /// Navigation property for origin location
    /// </summary>
    public Location OriginLocation { get; set; } = null!;

    /// <summary>
    /// Destination location ID
    /// </summary>
    public int DestinationLocationId { get; set; }

    /// <summary>
    /// Navigation property for destination location
    /// </summary>
    public Location DestinationLocation { get; set; } = null!;

    /// <summary>
    /// Carrier name
    /// </summary>
    public string Carrier { get; set; } = null!;

    /// <summary>
    /// Mode of transportation (e.g., Truck, Train, Ship)
    /// </summary>
    public string TransportMode { get; set; } = null!;

    /// <summary>
    /// Total quantity to be transported
    /// </summary>
    public decimal TotalQuantity { get; set; }

    /// <summary>
    /// Estimated cost per unit for transportation
    /// </summary>
    public decimal EstimatedCostPerUnit { get; set; }

    /// <summary>
    /// Total estimated cost for the transportation
    /// </summary>
    public decimal TotalEstimatedCost { get; set; }

    /// <summary>
    /// Actual cost of the transportation (if completed)
    /// </summary>
    public decimal? ActualCost { get; set; }

    /// <summary>
    /// Status of the transportation plan (e.g., Planned, In Transit, Completed)
    /// </summary>
    public string Status { get; set; } = null!;

    /// <summary>
    /// Additional notes about the transportation plan
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Collection of contract deliveries associated with this transportation plan
    /// </summary>
    public ICollection<ContractDelivery> ContractDeliveries { get; set; } = new List<ContractDelivery>();

    /// <summary>
    /// Collection of route segments in this transportation plan
    /// </summary>
    public ICollection<TransportationRoute> Routes { get; set; } = new List<TransportationRoute>();
}