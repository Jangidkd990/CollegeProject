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
public class ForecastController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IRepository<Forecast> _forecastRepository;
    private readonly IRepository<ForecastDetail> _forecastDetailRepository;
    private readonly ILogger<ForecastController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ForecastController(
        ApplicationDbContext dbContext,
        IRepository<Forecast> forecastRepository,
        IRepository<ForecastDetail> forecastDetailRepository,
        ILogger<ForecastController> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _forecastRepository = forecastRepository;
        _forecastDetailRepository = forecastDetailRepository;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    // GET: Forecast
    public async Task<IActionResult> Index()
    {
        var forecasts = await _dbContext.Forecasts
            .OrderByDescending(f => f.ForecastDate)
            .ToListAsync();

        return View(forecasts);
    }

    // GET: Forecast/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var forecast = await _dbContext.Forecasts
            .Include(f => _dbContext.ForecastDetails.Where(fd => fd.ForecastId == id))
            .FirstOrDefaultAsync(f => f.Id == id);

        if (forecast == null)
        {
            return NotFound();
        }

        var viewModel = MapToViewModel(forecast);

        // Get contracts and customers first
        var contracts = await _dbContext.Contracts.ToListAsync();
        var customers = await _dbContext.Customers.ToListAsync();
        
        // Then use in-memory data to create view models
        var forecastDetails = await _dbContext.ForecastDetails
            .Where(fd => fd.ForecastId == id)
            .ToListAsync();
            
        viewModel.Details = forecastDetails.Select(fd => {
            var contract = fd.ContractId.HasValue 
                ? contracts.FirstOrDefault(c => c.Id == fd.ContractId)
                : null;
            var customer = fd.CustomerId.HasValue
                ? customers.FirstOrDefault(c => c.Id == fd.CustomerId)
                : null;
                
            return new ForecastDetailViewModel
            {
                Id = fd.Id,
                ForecastId = fd.ForecastId,
                PeriodStart = fd.PeriodStart,
                PeriodEnd = fd.PeriodEnd,
                ContractId = fd.ContractId,
                ContractNumber = contract?.ContractNumber ?? "",
                CustomerId = fd.CustomerId,
                CustomerName = customer?.Name ?? "",
                ForecastValue = fd.ForecastValue,
                ActualValue = fd.ActualValue,
                ActualDate = fd.ActualDate,
                Notes = fd.Notes
            };
        }).ToList();

        return View(viewModel);
    }

    // GET: Forecast/Create
    public IActionResult Create()
    {
        var viewModel = new ForecastViewModel
        {
            ForecastDate = DateTime.Today,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System"
        };

        return View(viewModel);
    }

    // POST: Forecast/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ForecastViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var forecast = new Forecast
            {
                Title = viewModel.Title,
                ForecastDate = viewModel.ForecastDate,
                PeriodType = viewModel.PeriodType,
                Type = viewModel.Type,
                Description = viewModel.Description,
                IsActive = viewModel.IsActive,
                CreatedBy = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System",
                CreatedDate = DateTime.UtcNow
            };

            await _forecastRepository.AddAsync(forecast);

            _logger.LogInformation("Forecast {Title} created by {User}",
                forecast.Title, forecast.CreatedBy);

            return RedirectToAction(nameof(Index));
        }

        return View(viewModel);
    }

    // GET: Forecast/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var forecast = await _dbContext.Forecasts
            .Include(f => _dbContext.ForecastDetails.Where(fd => fd.ForecastId == id))
            .FirstOrDefaultAsync(f => f.Id == id);

        if (forecast == null)
        {
            return NotFound();
        }

        var viewModel = MapToViewModel(forecast);

        // Get contracts and customers first
        var contracts = await _dbContext.Contracts.ToListAsync();
        var customers = await _dbContext.Customers.ToListAsync();
        
        // Then use in-memory data to create view models
        var forecastDetails = await _dbContext.ForecastDetails
            .Where(fd => fd.ForecastId == id)
            .ToListAsync();
            
        viewModel.Details = forecastDetails.Select(fd => {
            var contract = fd.ContractId.HasValue 
                ? contracts.FirstOrDefault(c => c.Id == fd.ContractId)
                : null;
            var customer = fd.CustomerId.HasValue
                ? customers.FirstOrDefault(c => c.Id == fd.CustomerId)
                : null;
                
            return new ForecastDetailViewModel
            {
                Id = fd.Id,
                ForecastId = fd.ForecastId,
                PeriodStart = fd.PeriodStart,
                PeriodEnd = fd.PeriodEnd,
                ContractId = fd.ContractId,
                ContractNumber = contract?.ContractNumber ?? "",
                CustomerId = fd.CustomerId,
                CustomerName = customer?.Name ?? "",
                ForecastValue = fd.ForecastValue,
                ActualValue = fd.ActualValue,
                ActualDate = fd.ActualDate,
                Notes = fd.Notes
            };
        }).ToList();

        // Load dropdown options
        await LoadDetailDropdownOptions(viewModel.Details);

        return View(viewModel);
    }

    // POST: Forecast/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ForecastViewModel viewModel)
    {
        if (id != viewModel.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var forecast = await _forecastRepository.GetByIdAsync(id);
                if (forecast == null)
                {
                    return NotFound();
                }

                forecast.Title = viewModel.Title;
                forecast.ForecastDate = viewModel.ForecastDate;
                forecast.PeriodType = viewModel.PeriodType;
                forecast.Type = viewModel.Type;
                forecast.Description = viewModel.Description;
                forecast.IsActive = viewModel.IsActive;
                forecast.LastModifiedBy = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
                forecast.LastModifiedDate = DateTime.UtcNow;

                await _forecastRepository.UpdateAsync(forecast);

                _logger.LogInformation("Forecast {Title} updated by {User}",
                    forecast.Title, forecast.LastModifiedBy);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ForecastExists(id))
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

        return View(viewModel);
    }

    // GET: Forecast/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var forecast = await _dbContext.Forecasts
            .FirstOrDefaultAsync(f => f.Id == id);

        if (forecast == null)
        {
            return NotFound();
        }

        return View(MapToViewModel(forecast));
    }

    // POST: Forecast/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var forecast = await _forecastRepository.GetByIdAsync(id);
        if (forecast != null)
        {
            await _forecastRepository.DeleteAsync(forecast);

            _logger.LogInformation("Forecast {Title} deleted by {User}",
                forecast.Title, _httpContextAccessor.HttpContext?.User?.Identity?.Name);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Forecast/AddDetail/5
    public async Task<IActionResult> AddDetail(int id)
    {
        var forecast = await _forecastRepository.GetByIdAsync(id);
        if (forecast == null)
        {
            return NotFound();
        }

        var detailViewModel = new ForecastDetailViewModel
        {
            ForecastId = id,
            // Set default period dates based on forecast type
            PeriodStart = forecast.PeriodType switch
            {
                ForecastPeriodType.Monthly => new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                ForecastPeriodType.Quarterly => new DateTime(DateTime.Today.Year, ((DateTime.Today.Month - 1) / 3) * 3 + 1, 1),
                ForecastPeriodType.Yearly => new DateTime(DateTime.Today.Year, 1, 1),
                _ => DateTime.Today
            },
            PeriodEnd = forecast.PeriodType switch
            {
                ForecastPeriodType.Monthly => new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1),
                ForecastPeriodType.Quarterly => new DateTime(DateTime.Today.Year, ((DateTime.Today.Month - 1) / 3) * 3 + 1, 1).AddMonths(3).AddDays(-1),
                ForecastPeriodType.Yearly => new DateTime(DateTime.Today.Year, 12, 31),
                _ => DateTime.Today.AddMonths(1)
            }
        };

        // Load contract and customer lists
        var contracts = await _dbContext.Contracts
            .Where(c => c.IsActive)
            .OrderBy(c => c.ContractNumber)
            .ToListAsync();
            
        var customers = await _dbContext.Customers
            .OrderBy(c => c.Name)
            .ToListAsync();
            
        var contractOptions = new List<SelectListItem>();
        foreach (var contract in contracts)
        {
            var customer = customers.FirstOrDefault(c => c.Id == contract.CustomerId);
            contractOptions.Add(new SelectListItem
            {
                Value = contract.Id.ToString(),
                Text = $"{contract.ContractNumber} - {customer?.Name ?? "Unknown"}"
            });
        }
        
        detailViewModel.ContractOptions = contractOptions;
        detailViewModel.CustomerOptions = customers.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
        }).ToList();

        ViewBag.ForecastTitle = forecast.Title;

        return View(detailViewModel);
    }

    // POST: Forecast/AddDetail
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddDetail(ForecastDetailViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var detail = new ForecastDetail
            {
                ForecastId = viewModel.ForecastId,
                PeriodStart = viewModel.PeriodStart,
                PeriodEnd = viewModel.PeriodEnd,
                ContractId = viewModel.ContractId,
                CustomerId = viewModel.CustomerId,
                ForecastValue = viewModel.ForecastValue,
                ActualValue = viewModel.ActualValue,
                ActualDate = viewModel.ActualDate,
                Notes = viewModel.Notes
            };

            await _forecastDetailRepository.AddAsync(detail);

            _logger.LogInformation("Forecast detail added to forecast ID {ForecastId} by {User}",
                viewModel.ForecastId, _httpContextAccessor.HttpContext?.User?.Identity?.Name);

            return RedirectToAction(nameof(Edit), new { id = viewModel.ForecastId });
        }

        // If we got this far, something failed, reload dropdown options
        var contracts = await _dbContext.Contracts
            .Where(c => c.IsActive)
            .OrderBy(c => c.ContractNumber)
            .ToListAsync();
            
        var customers = await _dbContext.Customers
            .OrderBy(c => c.Name)
            .ToListAsync();
            
        var contractOptions = new List<SelectListItem>();
        foreach (var contract in contracts)
        {
            var customer = customers.FirstOrDefault(c => c.Id == contract.CustomerId);
            contractOptions.Add(new SelectListItem
            {
                Value = contract.Id.ToString(),
                Text = $"{contract.ContractNumber} - {customer?.Name ?? "Unknown"}"
            });
        }
        
        viewModel.ContractOptions = contractOptions;
        viewModel.CustomerOptions = customers.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
        }).ToList();

        var forecast = await _forecastRepository.GetByIdAsync(viewModel.ForecastId);
        ViewBag.ForecastTitle = forecast?.Title ?? "Forecast";

        return View(viewModel);
    }

    // GET: Forecast/EditDetail/5
    public async Task<IActionResult> EditDetail(int id)
    {
        var detail = await _forecastDetailRepository.GetByIdAsync(id);
        if (detail == null)
        {
            return NotFound();
        }

        var viewModel = new ForecastDetailViewModel
        {
            Id = detail.Id,
            ForecastId = detail.ForecastId,
            PeriodStart = detail.PeriodStart,
            PeriodEnd = detail.PeriodEnd,
            ContractId = detail.ContractId,
            CustomerId = detail.CustomerId,
            ForecastValue = detail.ForecastValue,
            ActualValue = detail.ActualValue,
            ActualDate = detail.ActualDate,
            Notes = detail.Notes
        };

        // Load contract and customer lists
        var contracts = await _dbContext.Contracts
            .Where(c => c.IsActive)
            .OrderBy(c => c.ContractNumber)
            .ToListAsync();
            
        var customers = await _dbContext.Customers
            .OrderBy(c => c.Name)
            .ToListAsync();
            
        var contractOptions = new List<SelectListItem>();
        foreach (var contract in contracts)
        {
            var customer = customers.FirstOrDefault(c => c.Id == contract.CustomerId);
            contractOptions.Add(new SelectListItem
            {
                Value = contract.Id.ToString(),
                Text = $"{contract.ContractNumber} - {customer?.Name ?? "Unknown"}"
            });
        }
        
        viewModel.ContractOptions = contractOptions;
        viewModel.CustomerOptions = customers.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
        }).ToList();

        var forecast = await _forecastRepository.GetByIdAsync(detail.ForecastId);
        ViewBag.ForecastTitle = forecast?.Title ?? "Forecast";

        return View(viewModel);
    }

    // POST: Forecast/EditDetail/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditDetail(int id, ForecastDetailViewModel viewModel)
    {
        if (id != viewModel.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var detail = await _forecastDetailRepository.GetByIdAsync(id);
                if (detail == null)
                {
                    return NotFound();
                }

                detail.PeriodStart = viewModel.PeriodStart;
                detail.PeriodEnd = viewModel.PeriodEnd;
                detail.ContractId = viewModel.ContractId;
                detail.CustomerId = viewModel.CustomerId;
                detail.ForecastValue = viewModel.ForecastValue;
                detail.ActualValue = viewModel.ActualValue;
                detail.ActualDate = viewModel.ActualDate;
                detail.Notes = viewModel.Notes;

                await _forecastDetailRepository.UpdateAsync(detail);

                _logger.LogInformation("Forecast detail ID {DetailId} updated by {User}",
                    detail.Id, _httpContextAccessor.HttpContext?.User?.Identity?.Name);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await DetailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Edit), new { id = viewModel.ForecastId });
        }

        // If validation failed, reload dropdown options
        var contracts = await _dbContext.Contracts
            .Where(c => c.IsActive)
            .OrderBy(c => c.ContractNumber)
            .ToListAsync();
            
        var customers = await _dbContext.Customers
            .OrderBy(c => c.Name)
            .ToListAsync();
            
        var contractOptions = new List<SelectListItem>();
        foreach (var contract in contracts)
        {
            var customer = customers.FirstOrDefault(c => c.Id == contract.CustomerId);
            contractOptions.Add(new SelectListItem
            {
                Value = contract.Id.ToString(),
                Text = $"{contract.ContractNumber} - {customer?.Name ?? "Unknown"}"
            });
        }
        
        viewModel.ContractOptions = contractOptions;
        viewModel.CustomerOptions = customers.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
        }).ToList();

        var forecast = await _forecastRepository.GetByIdAsync(viewModel.ForecastId);
        ViewBag.ForecastTitle = forecast?.Title ?? "Forecast";

        return View(viewModel);
    }

    // POST: Forecast/DeleteDetail/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDetail(int id, int forecastId)
    {
        var detail = await _forecastDetailRepository.GetByIdAsync(id);
        if (detail != null)
        {
            await _forecastDetailRepository.DeleteAsync(detail);

            _logger.LogInformation("Forecast detail ID {DetailId} deleted by {User}",
                id, _httpContextAccessor.HttpContext?.User?.Identity?.Name);
        }

        return RedirectToAction(nameof(Edit), new { id = forecastId });
    }

    // POST: Forecast/Approve/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Approve(int id)
    {
        var forecast = await _forecastRepository.GetByIdAsync(id);
        if (forecast == null)
        {
            return NotFound();
        }

        forecast.IsApproved = true;
        forecast.ApprovedBy = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        forecast.ApprovalDate = DateTime.UtcNow;

        await _forecastRepository.UpdateAsync(forecast);

        _logger.LogInformation("Forecast {Title} approved by {User}",
            forecast.Title, forecast.ApprovedBy);

        return RedirectToAction(nameof(Details), new { id });
    }

    // Helper methods
    private async Task<bool> ForecastExists(int id)
    {
        return await _dbContext.Forecasts.AnyAsync(f => f.Id == id);
    }

    private async Task<bool> DetailExists(int id)
    {
        return await _dbContext.ForecastDetails.AnyAsync(fd => fd.Id == id);
    }

    private ForecastViewModel MapToViewModel(Forecast forecast)
    {
        return new ForecastViewModel
        {
            Id = forecast.Id,
            Title = forecast.Title,
            ForecastDate = forecast.ForecastDate,
            PeriodType = forecast.PeriodType,
            Type = forecast.Type,
            Description = forecast.Description,
            IsActive = forecast.IsActive,
            IsApproved = forecast.IsApproved,
            ApprovedBy = forecast.ApprovedBy,
            ApprovalDate = forecast.ApprovalDate,
            CreatedBy = forecast.CreatedBy,
            CreatedDate = forecast.CreatedDate,
            LastModifiedBy = forecast.LastModifiedBy,
            LastModifiedDate = forecast.LastModifiedDate
        };
    }

    private async Task LoadDetailDropdownOptions(List<ForecastDetailViewModel> details)
    {
        var contracts = await _dbContext.Contracts
            .Where(c => c.IsActive)
            .OrderBy(c => c.ContractNumber)
            .ToListAsync();
            
        var customers = await _dbContext.Customers
            .OrderBy(c => c.Name)
            .ToListAsync();
            
        foreach (var detail in details)
        {
            var contract = detail.ContractId.HasValue
                ? contracts.FirstOrDefault(c => c.Id == detail.ContractId)
                : null;
                
            var customer = detail.CustomerId.HasValue
                ? customers.FirstOrDefault(c => c.Id == detail.CustomerId)
                : null;
                
            detail.ContractNumber = contract?.ContractNumber ?? string.Empty;
            detail.CustomerName = customer?.Name ?? string.Empty;
        }
    }
}