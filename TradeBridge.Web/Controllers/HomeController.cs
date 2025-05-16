using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeBridge.Core.Interfaces;
using TradeBridge.Data;
using TradeBridge.Web.Models;
using System.Diagnostics;

namespace TradeBridge.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly IRepository<TradeBridge.Core.Entities.Contract.Contract> _contractRepository;

    public HomeController(
        ILogger<HomeController> logger,
        ApplicationDbContext dbContext,
        IRepository<TradeBridge.Core.Entities.Contract.Contract> contractRepository)
    {
        _logger = logger;
        _dbContext = dbContext;
        _contractRepository = contractRepository;
    }

    public async Task<IActionResult> Index()
    {
        // Get dashboard summary data
        var dashboardModel = new DashboardViewModel
        {
            ContractCount = await _dbContext.Contracts.CountAsync(),
            ActiveContractCount = await _dbContext.Contracts.CountAsync(c => c.EndDate >= DateTime.Today && c.IsActive),
            PlannedDeliveryCount = await _dbContext.ContractDeliveries.CountAsync(d => d.ScheduledDate >= DateTime.Today),
            LocationCount = await _dbContext.Locations.CountAsync(),
            CustomerCount = await _dbContext.Customers.CountAsync(),
            SupplierCount = await _dbContext.Suppliers.CountAsync(),
            TransportationPlanCount = await _dbContext.TransportationPlans.CountAsync(),
            ProductionForecastCount = await _dbContext.ProductionForecasts.CountAsync(),
            ForecastCount = await _dbContext.Forecasts.CountAsync(),
            ActiveForecastCount = await _dbContext.Forecasts.CountAsync(f => f.IsActive),
            UF6BorrowingCount = await _dbContext.UF6BorrowingRecords.CountAsync(),
            PendingUF6RepaymentCount = await _dbContext.UF6BorrowingRecords.CountAsync(b => b.TransactionType == TradeBridge.Core.Entities.Forecasting.BorrowingTransactionType.Borrow && !b.IsSettled),
        };

        return View(dashboardModel);
    }

    [AllowAnonymous]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [AllowAnonymous]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
