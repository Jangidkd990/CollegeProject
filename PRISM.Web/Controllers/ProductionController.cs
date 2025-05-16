namespace PRISM.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PRISM.Core.Entities.Location;
using PRISM.Core.Entities.Production;
using PRISM.Core.Interfaces;
using System.Threading.Tasks;

[Authorize]
public class ProductionController : Controller
{
    private readonly IRepository<ProductionForecast> _productionForecastRepository;
    private readonly IRepository<Location> _locationRepository;
    private readonly IAuditService _auditService;

    public ProductionController(
        IRepository<ProductionForecast> productionForecastRepository,
        IRepository<Location> locationRepository,
        IAuditService auditService)
    {
        _productionForecastRepository = productionForecastRepository;
        _locationRepository = locationRepository;
        _auditService = auditService;
    }

    public async Task<IActionResult> Index()
    {
        var forecasts = await _productionForecastRepository.ListAllAsync();
        return View(forecasts);
    }

    public async Task<IActionResult> Details(int id)
    {
        var forecast = await _productionForecastRepository.GetByIdAsync(id);
        if (forecast == null)
        {
            return NotFound();
        }

        await _auditService.LogAuditAsync(
            User.Identity?.Name ?? "Unknown",
            nameof(ProductionForecast),
            id,
            AuditAction.Read);

        return View(forecast);
    }

    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create()
    {
        await PopulateDropDownLists();
        return View(new ProductionForecast
        {
            ForecastDate = DateTime.Today,
            ProductionDate = DateTime.Today.AddDays(1),
            IsActive = true
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create(ProductionForecast forecast)
    {
        if (ModelState.IsValid)
        {
            forecast.CreatedBy = User.Identity?.Name ?? "Unknown";
            
            // Calculate variance if actual quantity is provided
            if (forecast.ActualQuantity.HasValue)
            {
                CalculateVariance(forecast);
            }
            
            var result = await _productionForecastRepository.AddAsync(forecast);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(ProductionForecast),
                result.Id,
                AuditAction.Create);

            return RedirectToAction(nameof(Details), new { id = result.Id });
        }

        await PopulateDropDownLists(forecast);
        return View(forecast);
    }

    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id)
    {
        var forecast = await _productionForecastRepository.GetByIdAsync(id);
        if (forecast == null)
        {
            return NotFound();
        }

        await PopulateDropDownLists(forecast);
        return View(forecast);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id, ProductionForecast forecast)
    {
        if (id != forecast.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            forecast.ModifiedBy = User.Identity?.Name ?? "Unknown";
            forecast.ModifiedAt = DateTime.UtcNow;
            
            // Calculate variance if actual quantity is provided
            if (forecast.ActualQuantity.HasValue)
            {
                CalculateVariance(forecast);
            }
            
            await _productionForecastRepository.UpdateAsync(forecast);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(ProductionForecast),
                forecast.Id,
                AuditAction.Update);

            return RedirectToAction(nameof(Details), new { id = forecast.Id });
        }

        await PopulateDropDownLists(forecast);
        return View(forecast);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var forecast = await _productionForecastRepository.GetByIdAsync(id);
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
        var forecast = await _productionForecastRepository.GetByIdAsync(id);
        if (forecast != null)
        {
            forecast.ModifiedBy = User.Identity?.Name ?? "Unknown";
            await _productionForecastRepository.DeleteAsync(forecast);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(ProductionForecast),
                id,
                AuditAction.Delete);
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropDownLists(ProductionForecast? forecast = null)
    {
        var locations = await _locationRepository.ListAsync(l => l.IsActive);
        ViewBag.Locations = new SelectList(locations, "Id", "Name", forecast?.LocationId);
    }
    
    private void CalculateVariance(ProductionForecast forecast)
    {
        if (forecast.PlannedQuantity != 0 && forecast.ActualQuantity.HasValue)
        {
            forecast.VariancePercent = ((decimal)forecast.ActualQuantity.Value - forecast.PlannedQuantity) / forecast.PlannedQuantity * 100;
        }
        else
        {
            forecast.VariancePercent = 0;
        }
    }
}