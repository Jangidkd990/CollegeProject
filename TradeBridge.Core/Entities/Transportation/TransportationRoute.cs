namespace TradeBridge.Core.Entities.Transportation;

using TradeBridge.Core.Entities.Location;
using System;

/// <summary>
/// Represents a segment of a transportation route
/// </summary>
public class TransportationRoute : BaseEntity
{
    /// <summary>
    /// ID of the transportation plan this route belongs to
    /// </summary>
    public int TransportationPlanId { get; set; }

    /// <summary>
    /// Navigation property for the transportation plan
    /// </summary>
    public TransportationPlan TransportationPlan { get; set; } = null!;

    /// <summary>
    /// Sequence number of this route segment
    /// </summary>
    public int SequenceNumber { get; set; }

    /// <summary>
    /// ID of the starting location for this route segment
    /// </summary>
    public int FromLocationId { get; set; }

    /// <summary>
    /// Navigation property for the starting location
    /// </summary>
    public Location FromLocation { get; set; } = null!;

    /// <summary>
    /// ID of the ending location for this route segment
    /// </summary>
    public int ToLocationId { get; set; }

    /// <summary>
    /// Navigation property for the ending location
    /// </summary>
    public Location ToLocation { get; set; } = null!;

    /// <summary>
    /// Distance for this route segment (in miles)
    /// </summary>
    public decimal Distance { get; set; }

    /// <summary>
    /// Estimated transit time (in hours)
    /// </summary>
    public decimal EstimatedTransitTime { get; set; }

    /// <summary>
    /// Mode of transportation for this segment
    /// </summary>
    public string TransportMode { get; set; } = null!;

    /// <summary>
    /// Carrier for this route segment
    /// </summary>
    public string Carrier { get; set; } = null!;

    /// <summary>
    /// Estimated cost for this route segment
    /// </summary>
    public decimal EstimatedCost { get; set; }

    /// <summary>
    /// Actual cost for this route segment (if completed)
    /// </summary>
    public decimal? ActualCost { get; set; }

    /// <summary>
    /// Scheduled departure date/time
    /// </summary>
    public DateTime ScheduledDeparture { get; set; }

    /// <summary>
    /// Actual departure date/time (if departed)
    /// </summary>
    public DateTime? ActualDeparture { get; set; }

    /// <summary>
    /// Scheduled arrival date/time
    /// </summary>
    public DateTime ScheduledArrival { get; set; }

    /// <summary>
    /// Actual arrival date/time (if arrived)
    /// </summary>
    public DateTime? ActualArrival { get; set; }

    /// <summary>
    /// Status of this route segment (e.g., Scheduled, In Transit, Completed)
    /// </summary>
    public string Status { get; set; } = null!;

    /// <summary>
    /// Additional notes for this route segment
    /// </summary>
    public string? Notes { get; set; }
}