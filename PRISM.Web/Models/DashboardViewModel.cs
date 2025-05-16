namespace PRISM.Web.Models;

/// <summary>
/// View model for the dashboard
/// </summary>
public class DashboardViewModel
{
    /// <summary>
    /// Total number of contracts
    /// </summary>
    public int ContractCount { get; set; }

    /// <summary>
    /// Number of active contracts (not expired and active status)
    /// </summary>
    public int ActiveContractCount { get; set; }

    /// <summary>
    /// Number of deliveries planned for the future
    /// </summary>
    public int PlannedDeliveryCount { get; set; }

    /// <summary>
    /// Total number of locations
    /// </summary>
    public int LocationCount { get; set; }

    /// <summary>
    /// Total number of customers
    /// </summary>
    public int CustomerCount { get; set; }

    /// <summary>
    /// Total number of suppliers
    /// </summary>
    public int SupplierCount { get; set; }

    /// <summary>
    /// Total number of transportation plans
    /// </summary>
    public int TransportationPlanCount { get; set; }

    /// <summary>
    /// Total number of production forecasts
    /// </summary>
    public int ProductionForecastCount { get; set; }

    /// <summary>
    /// Total number of forecasts
    /// </summary>
    public int ForecastCount { get; set; }

    /// <summary>
    /// Number of active forecasts
    /// </summary>
    public int ActiveForecastCount { get; set; }

    /// <summary>
    /// Total number of UF6 borrowing records
    /// </summary>
    public int UF6BorrowingCount { get; set; }

    /// <summary>
    /// Number of UF6 borrowings that are not yet settled
    /// </summary>
    public int PendingUF6RepaymentCount { get; set; }
}