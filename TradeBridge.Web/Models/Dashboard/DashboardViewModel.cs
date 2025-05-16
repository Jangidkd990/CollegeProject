using TradeBridge.Core.Entities.Contract;
using TradeBridge.Core.Entities.Forecasting;

namespace TradeBridge.Web.Models.Dashboard;

public class DashboardViewModel
{
    // Contracts
    public int ActiveCustomerContracts { get; set; }
    public int ActiveSupplierContracts { get; set; }
    public decimal TotalContractValue { get; set; }
    public List<ContractDelivery> UpcomingDeliveries { get; set; } = new List<ContractDelivery>();

    // Borrowings
    public List<UF6BorrowingRecord> OutstandingBorrowings { get; set; } = new List<UF6BorrowingRecord>();
    public List<UF6BorrowingRecord> OverdueBorrowings { get; set; } = new List<UF6BorrowingRecord>();
    public decimal TotalBorrowedQuantity { get; set; }

    // Forecasts
    public int ActiveForecasts { get; set; }
    public ChartData? CurrentYearRevenueData { get; set; }
    public ChartData? PriceForecastData { get; set; }

    // Business Overview
    public int ActiveCustomers { get; set; }
    public int ActiveSuppliers { get; set; }
}

public class ChartData
{
    public string[] Labels { get; set; } = Array.Empty<string>();
    public decimal[] Values { get; set; } = Array.Empty<decimal>();
} 