using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeBridge.Core.Entities.Contract;
using TradeBridge.Core.Entities.Customer;
using TradeBridge.Core.Entities.Forecasting;
using TradeBridge.Core.Entities.Supplier;
using TradeBridge.Core.Enums;
using TradeBridge.Core.Interfaces;
using TradeBridge.Web.Models.Dashboard;

namespace TradeBridge.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IRepository<Contract> _contractRepository;
    private readonly IRepository<Forecast> _forecastRepository;
    private readonly IRepository<ForecastDetail> _forecastDetailRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<Supplier> _supplierRepository;
    private readonly IRepository<UF6BorrowingRecord> _borrowingRepository;

    public DashboardController(
        IRepository<Contract> contractRepository,
        IRepository<Forecast> forecastRepository,
        IRepository<ForecastDetail> forecastDetailRepository,
        IRepository<Customer> customerRepository,
        IRepository<Supplier> supplierRepository,
        IRepository<UF6BorrowingRecord> borrowingRepository)
    {
        _contractRepository = contractRepository;
        _forecastRepository = forecastRepository;
        _forecastDetailRepository = forecastDetailRepository;
        _customerRepository = customerRepository;
        _supplierRepository = supplierRepository;
        _borrowingRepository = borrowingRepository;
    }

    public async Task<IActionResult> Index()
    {
        var dashboardViewModel = new DashboardViewModel();

        await PopulateContractsSummary(dashboardViewModel);
        await PopulateBorrowingSummary(dashboardViewModel);
        await PopulateForecastSummary(dashboardViewModel);
        await PopulateBusinessOverview(dashboardViewModel);

        return View(dashboardViewModel);
    }

    private async Task PopulateContractsSummary(DashboardViewModel model)
    {
        var allContracts = await _contractRepository.ListAllAsync();
        var customerContracts = allContracts.Where(c => c.ContractType == ContractType.Customer && c.IsActive).ToList();
        var supplierContracts = allContracts.Where(c => c.ContractType == ContractType.Supplier && c.IsActive).ToList();

        model.ActiveCustomerContracts = customerContracts.Count;
        model.ActiveSupplierContracts = supplierContracts.Count;
        
        model.TotalContractValue = customerContracts.Sum(c => c.BaseQuantity * (c.FixedPrice ?? 0));
        
        model.UpcomingDeliveries = customerContracts
            .SelectMany(c => c.Deliveries)
            .Where(d => d.ScheduledDate > DateTime.Today && d.ScheduledDate <= DateTime.Today.AddDays(30))
            .OrderBy(d => d.ScheduledDate)
            .ToList();
    }

    private async Task PopulateBorrowingSummary(DashboardViewModel model)
    {
        var borrowingRecords = await _borrowingRepository.ListAllAsync();
        
        model.OutstandingBorrowings = borrowingRecords
            .Where(b => b.TransactionType == BorrowingTransactionType.Borrow && !b.IsSettled)
            .ToList();
        
        model.OverdueBorrowings = model.OutstandingBorrowings
            .Where(b => b.DueDate < DateTime.Today)
            .ToList();
        
        model.TotalBorrowedQuantity = model.OutstandingBorrowings.Sum(b => b.Quantity);
    }

    private async Task PopulateForecastSummary(DashboardViewModel model)
    {
        var forecasts = await _forecastRepository.ListAsync(f => f.IsActive);
        
        model.ActiveForecasts = forecasts.Count;
        
        var revenueForecast = forecasts.FirstOrDefault(f => f.Type == ForecastType.Revenue);
        if (revenueForecast != null)
        {
            var revenueDetails = await _forecastDetailRepository.ListAsync(d => d.ForecastId == revenueForecast.Id);
            
            model.CurrentYearRevenueData = new ChartData
            {
                Labels = new[] { "Q1", "Q2", "Q3", "Q4" },
                Values = new decimal[4]
            };
            
            foreach (var detail in revenueDetails)
            {
                var quarter = (detail.PeriodStart.Month - 1) / 3;
                if (quarter >= 0 && quarter < 4 && detail.PeriodStart.Year == DateTime.Today.Year)
                {
                    model.CurrentYearRevenueData.Values[quarter] += detail.ForecastValue;
                }
            }
        }
        
        var priceForecast = forecasts.FirstOrDefault(f => f.Type == ForecastType.Price);
        if (priceForecast != null)
        {
            var priceDetails = await _forecastDetailRepository.ListAsync(d => d.ForecastId == priceForecast.Id);
            
            var priceData = priceDetails
                .Where(d => d.PeriodStart.Year == DateTime.Today.Year || d.PeriodStart.Year == DateTime.Today.Year + 1)
                .OrderBy(d => d.PeriodStart)
                .ToList();
            
            model.PriceForecastData = new ChartData
            {
                Labels = priceData.Select(d => $"{d.PeriodStart:MMM yy}").ToArray(),
                Values = priceData.Select(d => d.ForecastValue).ToArray()
            };
        }
    }

    private async Task PopulateBusinessOverview(DashboardViewModel model)
    {
        var customers = await _customerRepository.ListAsync(c => c.IsActive);
        var suppliers = await _supplierRepository.ListAsync(s => s.IsActive);
        
        model.ActiveCustomers = customers.Count;
        model.ActiveSuppliers = suppliers.Count;
    }
} 