namespace PRISM.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PRISM.Core.Entities.Contract;
using PRISM.Core.Entities.Customer;
using PRISM.Core.Entities.Forecasting;
using PRISM.Core.Enums;
using PRISM.Core.Interfaces;
using System.Threading.Tasks;

[Authorize]
public class ForecastsController : Controller
{
    private readonly IRepository<Forecast> _forecastRepository;
    private readonly IRepository<ForecastDetail> _forecastDetailRepository;
    private readonly IRepository<Contract> _contractRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<UF6BorrowingRecord> _borrowingRepository;
    private readonly IAuditService _auditService;

    public ForecastsController(
        IRepository<Forecast> forecastRepository,
        IRepository<ForecastDetail> forecastDetailRepository,
        IRepository<Contract> contractRepository,
        IRepository<Customer> customerRepository,
        IRepository<UF6BorrowingRecord> borrowingRepository,
        IAuditService auditService)
    {
        _forecastRepository = forecastRepository;
        _forecastDetailRepository = forecastDetailRepository;
        _contractRepository = contractRepository;
        _customerRepository = customerRepository;
        _borrowingRepository = borrowingRepository;
        _auditService = auditService;
    }

    public async Task<IActionResult> Index()
    {
        var forecasts = await _forecastRepository.ListAllAsync();
        return View(forecasts);
    }

    public async Task<IActionResult> Details(int id)
    {
        var forecast = await _forecastRepository.GetByIdAsync(id);
        if (forecast == null)
        {
            return NotFound();
        }

        // Get all forecast details for this forecast
        var details = await _forecastDetailRepository.ListAsync(d => d.ForecastId == id);

        ViewBag.ForecastDetails = details;

        await _auditService.LogAuditAsync(
            User.Identity?.Name ?? "Unknown",
            nameof(Forecast),
            id,
            AuditAction.Read);

        return View(forecast);
    }

    [Authorize(Roles = "Admin,Manager")]
    public IActionResult Create()
    {
        PopulateForecastDropDowns();
        return View(new Forecast
        {
            ForecastDate = DateTime.Today,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create(Forecast forecast)
    {
        if (ModelState.IsValid)
        {
            forecast.CreatedBy = User.Identity?.Name ?? "Unknown";
            forecast.CreatedDate = DateTime.UtcNow;
            var result = await _forecastRepository.AddAsync(forecast);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(Forecast),
                result.Id,
                AuditAction.Create);

            return RedirectToAction(nameof(Details), new { id = result.Id });
        }

        PopulateForecastDropDowns();
        return View(forecast);
    }

    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id)
    {
        var forecast = await _forecastRepository.GetByIdAsync(id);
        if (forecast == null)
        {
            return NotFound();
        }

        PopulateForecastDropDowns();
        return View(forecast);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id, Forecast forecast)
    {
        if (id != forecast.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            forecast.LastModifiedBy = User.Identity?.Name ?? "Unknown";
            forecast.LastModifiedDate = DateTime.UtcNow;
            await _forecastRepository.UpdateAsync(forecast);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(Forecast),
                forecast.Id,
                AuditAction.Update);

            return RedirectToAction(nameof(Details), new { id = forecast.Id });
        }

        PopulateForecastDropDowns();
        return View(forecast);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var forecast = await _forecastRepository.GetByIdAsync(id);
        if (forecast == null)
        {
            return NotFound();
        }

        return View(forecast);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var forecast = await _forecastRepository.GetByIdAsync(id);
        if (forecast != null)
        {
            forecast.ModifiedBy = User.Identity?.Name ?? "Unknown";
            await _forecastRepository.DeleteAsync(forecast);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(Forecast),
                id,
                AuditAction.Delete);
        }

        return RedirectToAction(nameof(Index));
    }

    // Forecast Detail Management
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AddDetail(int forecastId)
    {
        var forecast = await _forecastRepository.GetByIdAsync(forecastId);
        if (forecast == null)
        {
            return NotFound();
        }

        ViewBag.ForecastId = forecastId;
        ViewBag.ForecastTitle = forecast.Title;
        await PopulateDetailDropDowns();

        var startDate = DateTime.Today.AddDays(1);
        var endDate = GetEndDateBasedOnPeriodType(startDate, forecast.PeriodType);

        return View(new ForecastDetail
        {
            ForecastId = forecastId,
            PeriodStart = startDate,
            PeriodEnd = endDate,
            ForecastValue = 0
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AddDetail(ForecastDetail detail)
    {
        if (ModelState.IsValid)
        {
            detail.CreatedBy = User.Identity?.Name ?? "Unknown";
            await _forecastDetailRepository.AddAsync(detail);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(ForecastDetail),
                detail.Id,
                AuditAction.Create);

            return RedirectToAction(nameof(Details), new { id = detail.ForecastId });
        }

        var forecast = await _forecastRepository.GetByIdAsync(detail.ForecastId);
        ViewBag.ForecastId = detail.ForecastId;
        ViewBag.ForecastTitle = forecast?.Title ?? "Unknown Forecast";
        await PopulateDetailDropDowns();

        return View(detail);
    }

    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> EditDetail(int id)
    {
        var detail = await _forecastDetailRepository.GetByIdAsync(id);
        if (detail == null)
        {
            return NotFound();
        }

        var forecast = await _forecastRepository.GetByIdAsync(detail.ForecastId);
        ViewBag.ForecastId = detail.ForecastId;
        ViewBag.ForecastTitle = forecast?.Title ?? "Unknown Forecast";
        await PopulateDetailDropDowns(detail.ContractId, detail.CustomerId);

        return View(detail);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> EditDetail(int id, ForecastDetail detail)
    {
        if (id != detail.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            detail.ModifiedBy = User.Identity?.Name ?? "Unknown";
            detail.ModifiedAt = DateTime.UtcNow;
            await _forecastDetailRepository.UpdateAsync(detail);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(ForecastDetail),
                detail.Id,
                AuditAction.Update);

            return RedirectToAction(nameof(Details), new { id = detail.ForecastId });
        }

        var forecast = await _forecastRepository.GetByIdAsync(detail.ForecastId);
        ViewBag.ForecastId = detail.ForecastId;
        ViewBag.ForecastTitle = forecast?.Title ?? "Unknown Forecast";
        await PopulateDetailDropDowns(detail.ContractId, detail.CustomerId);

        return View(detail);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteDetail(int id)
    {
        var detail = await _forecastDetailRepository.GetByIdAsync(id);
        if (detail == null)
        {
            return NotFound();
        }

        var forecastId = detail.ForecastId;

        detail.ModifiedBy = User.Identity?.Name ?? "Unknown";
        await _forecastDetailRepository.DeleteAsync(detail);

        await _auditService.LogAuditAsync(
            User.Identity?.Name ?? "Unknown",
            nameof(ForecastDetail),
            id,
            AuditAction.Delete);

        return RedirectToAction(nameof(Details), new { id = forecastId });
    }

    // UF6 Borrowing Record Management
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AddBorrowingRecord()
    {
        await PopulateBorrowingDropDowns();
        return View(new UF6BorrowingRecord
        {
            TransactionDate = DateTime.Today,
            DueDate = DateTime.Today.AddMonths(3),
            TransactionType = BorrowingTransactionType.Borrow,
            Unit = "KgU",
            Quantity = 0
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AddBorrowingRecord(UF6BorrowingRecord borrowing)
    {
        if (ModelState.IsValid)
        {
            borrowing.CreatedBy = User.Identity?.Name ?? "Unknown";
            await _borrowingRepository.AddAsync(borrowing);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(UF6BorrowingRecord),
                borrowing.Id,
                AuditAction.Create);

            return RedirectToAction(nameof(BorrowingRecords));
        }

        await PopulateBorrowingDropDowns(borrowing.ContractId, borrowing.CustomerId);
        return View(borrowing);
    }

    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> EditBorrowingRecord(int id)
    {
        var borrowing = await _borrowingRepository.GetByIdAsync(id);
        if (borrowing == null)
        {
            return NotFound();
        }

        await PopulateBorrowingDropDowns(borrowing.ContractId, borrowing.CustomerId);
        return View(borrowing);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> EditBorrowingRecord(int id, UF6BorrowingRecord borrowing)
    {
        if (id != borrowing.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            borrowing.ModifiedBy = User.Identity?.Name ?? "Unknown";
            borrowing.ModifiedAt = DateTime.UtcNow;
            await _borrowingRepository.UpdateAsync(borrowing);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(UF6BorrowingRecord),
                borrowing.Id,
                AuditAction.Update);

            return RedirectToAction(nameof(BorrowingRecords));
        }

        await PopulateBorrowingDropDowns(borrowing.ContractId, borrowing.CustomerId);
        return View(borrowing);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteBorrowingRecord(int id)
    {
        var borrowing = await _borrowingRepository.GetByIdAsync(id);
        if (borrowing == null)
        {
            return NotFound();
        }

        borrowing.ModifiedBy = User.Identity?.Name ?? "Unknown";
        await _borrowingRepository.DeleteAsync(borrowing);

        await _auditService.LogAuditAsync(
            User.Identity?.Name ?? "Unknown",
            nameof(UF6BorrowingRecord),
            id,
            AuditAction.Delete);

        return RedirectToAction(nameof(BorrowingRecords));
    }

    [Authorize]
    public async Task<IActionResult> BorrowingRecords()
    {
        var borrowings = await _borrowingRepository.ListAllAsync();
        return View(borrowings);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ApproveForecast(int id)
    {
        var forecast = await _forecastRepository.GetByIdAsync(id);
        if (forecast == null)
        {
            return NotFound();
        }
        
        if (forecast.IsApproved)
        {
            // Already approved
            return RedirectToAction(nameof(Details), new { id });
        }
        
        forecast.IsApproved = true;
        forecast.ApprovedBy = User.Identity?.Name ?? "Unknown";
        forecast.ApprovalDate = DateTime.UtcNow;
        forecast.ModifiedBy = User.Identity?.Name ?? "Unknown";
        forecast.ModifiedAt = DateTime.UtcNow;
        
        await _forecastRepository.UpdateAsync(forecast);
        
        await _auditService.LogAuditAsync(
            User.Identity?.Name ?? "Unknown",
            nameof(Forecast),
            id,
            AuditAction.Update,
            "Forecast approved");
            
        return RedirectToAction(nameof(Details), new { id });
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetBorrowRecords(int contractId)
    {
        // Get borrow transactions for this contract that aren't settled yet
        var borrowRecords = await _borrowingRepository.ListAsync(b => 
            b.ContractId == contractId && 
            b.TransactionType == BorrowingTransactionType.Borrow && 
            !b.IsSettled);
            
        // Return as JSON
        return Json(borrowRecords.Select(b => new {
            id = b.Id,
            transactionDate = b.TransactionDate,
            quantity = b.Quantity,
            unit = b.Unit,
            dueDate = b.DueDate
        }));
    }

    // Helper Methods
    private void PopulateForecastDropDowns()
    {
        ViewBag.PeriodTypes = new SelectList(Enum.GetValues(typeof(ForecastPeriodType))
            .Cast<ForecastPeriodType>()
            .Select(t => new { Id = (int)t, Name = t.ToString() }),
            "Id", "Name");

        ViewBag.ForecastTypes = new SelectList(Enum.GetValues(typeof(ForecastType))
            .Cast<ForecastType>()
            .Select(t => new { Id = (int)t, Name = t.ToString() }),
            "Id", "Name");
    }

    private async Task PopulateDetailDropDowns(int? selectedContractId = null, int? selectedCustomerId = null)
    {
        var contracts = await _contractRepository.ListAsync(c => c.IsActive);
        var customers = await _customerRepository.ListAsync(c => c.IsActive);

        ViewBag.Contracts = new SelectList(contracts, "Id", "ContractNumber", selectedContractId);
        ViewBag.Customers = new SelectList(customers, "Id", "Name", selectedCustomerId);
    }

    private async Task PopulateBorrowingDropDowns(int? selectedContractId = null, int? selectedCustomerId = null)
    {
        var contracts = await _contractRepository.ListAsync(c => c.IsActive && c.IsBorrowingAllowed);
        var customers = await _customerRepository.ListAsync(c => c.IsActive);

        ViewBag.Contracts = new SelectList(contracts, "Id", "ContractNumber", selectedContractId);
        ViewBag.Customers = new SelectList(customers, "Id", "Name", selectedCustomerId);
        ViewBag.TransactionTypes = new SelectList(Enum.GetValues(typeof(BorrowingTransactionType))
            .Cast<BorrowingTransactionType>()
            .Select(t => new { Id = (int)t, Name = t.ToString() }),
            "Id", "Name");
    }

    private DateTime GetEndDateBasedOnPeriodType(DateTime startDate, ForecastPeriodType periodType)
    {
        return periodType switch
        {
            ForecastPeriodType.Monthly => startDate.AddMonths(1).AddDays(-1),
            ForecastPeriodType.Quarterly => startDate.AddMonths(3).AddDays(-1),
            ForecastPeriodType.Yearly => startDate.AddYears(1).AddDays(-1),
            _ => startDate.AddMonths(1).AddDays(-1)
        };
    }
} 