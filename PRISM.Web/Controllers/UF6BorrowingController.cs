using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PRISM.Core.Entities.Forecasting;
using PRISM.Core.Interfaces;
using PRISM.Data;
using PRISM.Web.Models.Forecasting;

namespace PRISM.Web.Controllers;

[Authorize]
public class UF6BorrowingController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IRepository<UF6BorrowingRecord> _borrowingRepository;
    private readonly ILogger<UF6BorrowingController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UF6BorrowingController(
        ApplicationDbContext dbContext,
        IRepository<UF6BorrowingRecord> borrowingRepository,
        ILogger<UF6BorrowingController> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _borrowingRepository = borrowingRepository;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    // GET: UF6Borrowing
    public async Task<IActionResult> Index()
    {
        var borrowingRecords = await _dbContext.UF6BorrowingRecords
            .OrderByDescending(r => r.TransactionDate)
            .ToListAsync();
            
        var contracts = await _dbContext.Contracts.ToListAsync();
        var customers = await _dbContext.Customers.ToListAsync();
            
        var viewModels = borrowingRecords.Select(r => new UF6BorrowingViewModel
        {
            Id = r.Id,
            ContractId = r.ContractId,
            ContractNumber = contracts.FirstOrDefault(c => c.Id == r.ContractId)?.ContractNumber ?? string.Empty,
            CustomerId = r.CustomerId,
            CustomerName = r.CustomerId.HasValue 
                ? customers.FirstOrDefault(c => c.Id == r.CustomerId)?.Name ?? string.Empty 
                : string.Empty,
            TransactionType = r.TransactionType,
            TransactionDate = r.TransactionDate,
            Quantity = r.Quantity,
            Unit = r.Unit,
            DueDate = r.DueDate,
            IsSettled = r.IsSettled,
            SettlementDate = r.SettlementDate,
            Notes = r.Notes,
            RelatedRecordId = r.RelatedRecordId
        }).ToList();

        return View(viewModels);
    }

    // GET: UF6Borrowing/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var record = await _dbContext.UF6BorrowingRecords
            .FirstOrDefaultAsync(r => r.Id == id);

        if (record == null)
        {
            return NotFound();
        }

        var viewModel = MapToViewModel(record);

        // Load related transaction details if available
        if (record.RelatedRecordId.HasValue)
        {
            var relatedRecord = await _dbContext.UF6BorrowingRecords
                .FirstOrDefaultAsync(r => r.Id == record.RelatedRecordId);

            if (relatedRecord != null)
            {
                viewModel.RelatedTransactionDetails = $"{relatedRecord.TransactionType} on {relatedRecord.TransactionDate:d} for {relatedRecord.Quantity} {relatedRecord.Unit}";
            }
        }

        return View(viewModel);
    }

    // GET: UF6Borrowing/Create
    public async Task<IActionResult> Create()
    {
        var viewModel = new UF6BorrowingViewModel
        {
            TransactionDate = DateTime.Today,
            DueDate = DateTime.Today.AddMonths(3)
        };

        await LoadDropdownOptions(viewModel);

        return View(viewModel);
    }

    // POST: UF6Borrowing/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UF6BorrowingViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var record = new UF6BorrowingRecord
            {
                ContractId = viewModel.ContractId,
                CustomerId = viewModel.CustomerId,
                TransactionType = viewModel.TransactionType,
                TransactionDate = viewModel.TransactionDate,
                Quantity = viewModel.Quantity,
                Unit = viewModel.Unit,
                DueDate = viewModel.DueDate,
                IsSettled = viewModel.IsSettled,
                SettlementDate = viewModel.SettlementDate,
                Notes = viewModel.Notes,
                RelatedRecordId = viewModel.RelatedRecordId
            };

            await _borrowingRepository.AddAsync(record);

            _logger.LogInformation("UF6 {TransactionType} record created for contract {ContractId} by {User}",
                record.TransactionType, record.ContractId,
                _httpContextAccessor.HttpContext?.User?.Identity?.Name);

            return RedirectToAction(nameof(Index));
        }

        await LoadDropdownOptions(viewModel);
        return View(viewModel);
    }

    // GET: UF6Borrowing/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var record = await _borrowingRepository.GetByIdAsync(id);
        if (record == null)
        {
            return NotFound();
        }

        var viewModel = MapToViewModel(record);
        await LoadDropdownOptions(viewModel);

        return View(viewModel);
    }

    // POST: UF6Borrowing/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UF6BorrowingViewModel viewModel)
    {
        if (id != viewModel.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var record = await _borrowingRepository.GetByIdAsync(id);
                if (record == null)
                {
                    return NotFound();
                }

                record.ContractId = viewModel.ContractId;
                record.CustomerId = viewModel.CustomerId;
                record.TransactionType = viewModel.TransactionType;
                record.TransactionDate = viewModel.TransactionDate;
                record.Quantity = viewModel.Quantity;
                record.Unit = viewModel.Unit;
                record.DueDate = viewModel.DueDate;
                record.IsSettled = viewModel.IsSettled;
                record.SettlementDate = viewModel.SettlementDate;
                record.Notes = viewModel.Notes;
                record.RelatedRecordId = viewModel.RelatedRecordId;

                await _borrowingRepository.UpdateAsync(record);

                _logger.LogInformation("UF6 {TransactionType} record updated for contract {ContractId} by {User}",
                    record.TransactionType, record.ContractId,
                    _httpContextAccessor.HttpContext?.User?.Identity?.Name);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await RecordExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        await LoadDropdownOptions(viewModel);
        return View(viewModel);
    }

    // GET: UF6Borrowing/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var record = await _dbContext.UF6BorrowingRecords
            .FirstOrDefaultAsync(r => r.Id == id);

        if (record == null)
        {
            return NotFound();
        }

        var viewModel = MapToViewModel(record);

        return View(viewModel);
    }

    // POST: UF6Borrowing/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var record = await _borrowingRepository.GetByIdAsync(id);
        if (record != null)
        {
            await _borrowingRepository.DeleteAsync(record);

            _logger.LogInformation("UF6 {TransactionType} record deleted for contract {ContractId} by {User}",
                record.TransactionType, record.ContractId,
                _httpContextAccessor.HttpContext?.User?.Identity?.Name);
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: UF6Borrowing/CreateRepayment/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRepayment(int id)
    {
        var borrowRecord = await _borrowingRepository.GetByIdAsync(id);
        if (borrowRecord == null)
        {
            return NotFound();
        }

        if (borrowRecord.TransactionType != BorrowingTransactionType.Borrow)
        {
            return BadRequest("Can only create repayments for records with 'Borrow' transaction type");
        }

        var repaymentRecord = new UF6BorrowingRecord
        {
            ContractId = borrowRecord.ContractId,
            CustomerId = borrowRecord.CustomerId,
            TransactionType = BorrowingTransactionType.Repay,
            TransactionDate = DateTime.Today,
            Quantity = borrowRecord.Quantity,
            Unit = borrowRecord.Unit,
            DueDate = borrowRecord.DueDate,
            IsSettled = true,
            SettlementDate = DateTime.Today,
            Notes = $"Repayment for borrowing record #{borrowRecord.Id}",
            RelatedRecordId = borrowRecord.Id
        };

        await _borrowingRepository.AddAsync(repaymentRecord);

        // Mark the original borrowing as settled
        borrowRecord.IsSettled = true;
        borrowRecord.SettlementDate = DateTime.Today;
        await _borrowingRepository.UpdateAsync(borrowRecord);

        _logger.LogInformation("UF6 Repayment record created for contract {ContractId} by {User}",
            repaymentRecord.ContractId, _httpContextAccessor.HttpContext?.User?.Identity?.Name);

        return RedirectToAction(nameof(Index));
    }

    // POST: UF6Borrowing/CreateDeferral/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateDeferral(int id, DateTime newDueDate)
    {
        var borrowRecord = await _borrowingRepository.GetByIdAsync(id);
        if (borrowRecord == null)
        {
            return NotFound();
        }

        if (borrowRecord.IsSettled)
        {
            return BadRequest("Cannot defer a settled record");
        }

        var deferralRecord = new UF6BorrowingRecord
        {
            ContractId = borrowRecord.ContractId,
            CustomerId = borrowRecord.CustomerId,
            TransactionType = BorrowingTransactionType.Defer,
            TransactionDate = DateTime.Today,
            Quantity = borrowRecord.Quantity,
            Unit = borrowRecord.Unit,
            DueDate = newDueDate,
            IsSettled = false,
            Notes = $"Deferral for borrowing record #{borrowRecord.Id}",
            RelatedRecordId = borrowRecord.Id
        };

        await _borrowingRepository.AddAsync(deferralRecord);

        // Mark the original borrowing as settled
        borrowRecord.IsSettled = true;
        borrowRecord.SettlementDate = DateTime.Today;
        await _borrowingRepository.UpdateAsync(borrowRecord);

        _logger.LogInformation("UF6 Deferral record created for contract {ContractId} by {User}",
            deferralRecord.ContractId, _httpContextAccessor.HttpContext?.User?.Identity?.Name);

        return RedirectToAction(nameof(Index));
    }

    // Helper methods
    private async Task<bool> RecordExists(int id)
    {
        return await _dbContext.UF6BorrowingRecords.AnyAsync(r => r.Id == id);
    }

    private UF6BorrowingViewModel MapToViewModel(UF6BorrowingRecord record)
    {
        var contract = _dbContext.Contracts.FirstOrDefault(c => c.Id == record.ContractId);
        var contractNumber = contract != null ? contract.ContractNumber : string.Empty;
        
        string customerName = string.Empty;
        if (record.CustomerId.HasValue)
        {
            var customer = _dbContext.Customers.FirstOrDefault(c => c.Id == record.CustomerId);
            customerName = customer != null ? customer.Name : string.Empty;
        }
        
        return new UF6BorrowingViewModel
        {
            Id = record.Id,
            ContractId = record.ContractId,
            ContractNumber = contractNumber,
            CustomerId = record.CustomerId,
            CustomerName = customerName,
            TransactionType = record.TransactionType,
            TransactionDate = record.TransactionDate,
            Quantity = record.Quantity,
            Unit = record.Unit,
            DueDate = record.DueDate,
            IsSettled = record.IsSettled,
            SettlementDate = record.SettlementDate,
            Notes = record.Notes,
            RelatedRecordId = record.RelatedRecordId
        };
    }

    private async Task LoadDropdownOptions(UF6BorrowingViewModel viewModel)
    {
        // Load contracts and customers
        var contracts = await _dbContext.Contracts
            .Where(c => c.IsActive)
            .OrderBy(c => c.ContractNumber)
            .ToListAsync();
            
        var customers = await _dbContext.Customers
            .OrderBy(c => c.Name)
            .ToListAsync();
            
        // Create the contract dropdown items
        var contractOptions = new List<SelectListItem>();
        foreach (var contract in contracts)
        {
            var customer = customers.FirstOrDefault(c => c.Id == contract.CustomerId);
            var customerName = customer != null ? customer.Name : "Unknown Customer";
            
            contractOptions.Add(new SelectListItem
            {
                Value = contract.Id.ToString(),
                Text = $"{contract.ContractNumber} - {customerName}"
            });
        }
        viewModel.ContractOptions = contractOptions;
        
        // Load customers
        viewModel.CustomerOptions = customers.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
        }).ToList();

        // Load related records (for repayments and deferrals)
        viewModel.RelatedRecordOptions = await _dbContext.UF6BorrowingRecords
            .Where(r => r.TransactionType == BorrowingTransactionType.Borrow && !r.IsSettled)
            .OrderByDescending(r => r.TransactionDate)
            .Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = $"ID: {r.Id} - {r.TransactionDate:d} - {r.Quantity} {r.Unit} - Due: {r.DueDate:d}"
            })
            .ToListAsync();
    }
}