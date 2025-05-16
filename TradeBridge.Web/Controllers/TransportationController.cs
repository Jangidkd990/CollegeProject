namespace TradeBridge.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TradeBridge.Core.Entities.Location;
using TradeBridge.Core.Entities.Transportation;
using TradeBridge.Core.Interfaces;
using System.Threading.Tasks;

[Authorize]
public class TransportationController : Controller
{
    private readonly IRepository<TransportationPlan> _planRepository;
    private readonly IRepository<TransportationRoute> _routeRepository;
    private readonly IRepository<ShippingRate> _rateRepository;
    private readonly IRepository<Location> _locationRepository;
    private readonly IAuditService _auditService;

    public TransportationController(
        IRepository<TransportationPlan> planRepository,
        IRepository<TransportationRoute> routeRepository,
        IRepository<ShippingRate> rateRepository,
        IRepository<Location> locationRepository,
        IAuditService auditService)
    {
        _planRepository = planRepository;
        _routeRepository = routeRepository;
        _rateRepository = rateRepository;
        _locationRepository = locationRepository;
        _auditService = auditService;
    }

    #region Transportation Plans

    public async Task<IActionResult> Index()
    {
        var plans = await _planRepository.ListAllAsync();
        return View(plans);
    }

    public async Task<IActionResult> Details(int id)
    {
        var plan = await _planRepository.GetByIdAsync(id);
        if (plan == null)
        {
            return NotFound();
        }

        await _auditService.LogAuditAsync(
            User.Identity?.Name ?? "Unknown",
            nameof(TransportationPlan),
            id,
            AuditAction.Read);

        return View(plan);
    }

    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create()
    {
        await PopulateLocationDropdownsAsync();
        return View(new TransportationPlan
        {
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(7)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create(TransportationPlan plan)
    {
        if (ModelState.IsValid)
        {
            plan.CreatedBy = User.Identity?.Name ?? "Unknown";

            // Calculate total estimated cost
            plan.TotalEstimatedCost = plan.TotalQuantity * plan.EstimatedCostPerUnit;

            var result = await _planRepository.AddAsync(plan);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(TransportationPlan),
                result.Id,
                AuditAction.Create);

            return RedirectToAction(nameof(Details), new { id = result.Id });
        }

        await PopulateLocationDropdownsAsync();
        return View(plan);
    }

    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id)
    {
        var plan = await _planRepository.GetByIdAsync(id);
        if (plan == null)
        {
            return NotFound();
        }

        await PopulateLocationDropdownsAsync();
        return View(plan);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id, TransportationPlan plan)
    {
        if (id != plan.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            plan.ModifiedBy = User.Identity?.Name ?? "Unknown";
            plan.ModifiedAt = DateTime.UtcNow;

            // Recalculate total estimated cost
            plan.TotalEstimatedCost = plan.TotalQuantity * plan.EstimatedCostPerUnit;

            await _planRepository.UpdateAsync(plan);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(TransportationPlan),
                plan.Id,
                AuditAction.Update);

            return RedirectToAction(nameof(Details), new { id = plan.Id });
        }

        await PopulateLocationDropdownsAsync();
        return View(plan);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var plan = await _planRepository.GetByIdAsync(id);
        if (plan == null)
        {
            return NotFound();
        }

        return View(plan);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var plan = await _planRepository.GetByIdAsync(id);
        if (plan != null)
        {
            plan.ModifiedBy = User.Identity?.Name ?? "Unknown";
            await _planRepository.DeleteAsync(plan);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(TransportationPlan),
                id,
                AuditAction.Delete);
        }

        return RedirectToAction(nameof(Index));
    }

    #endregion

    #region Shipping Rates

    public async Task<IActionResult> ShippingRates()
    {
        var rates = await _rateRepository.ListAllAsync();
        return View(rates);
    }

    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateRate()
    {
        await PopulateLocationDropdownsAsync();
        return View(new ShippingRate
        {
            EffectiveDate = DateTime.Today,
            FuelSurchargePercent = 0
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateRate(ShippingRate rate)
    {
        if (ModelState.IsValid)
        {
            rate.CreatedBy = User.Identity?.Name ?? "Unknown";
            var result = await _rateRepository.AddAsync(rate);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(ShippingRate),
                result.Id,
                AuditAction.Create);

            return RedirectToAction(nameof(ShippingRates));
        }

        await PopulateLocationDropdownsAsync();
        return View(rate);
    }

    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> EditRate(int id)
    {
        var rate = await _rateRepository.GetByIdAsync(id);
        if (rate == null)
        {
            return NotFound();
        }

        await PopulateLocationDropdownsAsync();
        return View(rate);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> EditRate(int id, ShippingRate rate)
    {
        if (id != rate.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            rate.ModifiedBy = User.Identity?.Name ?? "Unknown";
            rate.ModifiedAt = DateTime.UtcNow;
            await _rateRepository.UpdateAsync(rate);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(ShippingRate),
                rate.Id,
                AuditAction.Update);

            return RedirectToAction(nameof(ShippingRates));
        }

        await PopulateLocationDropdownsAsync();
        return View(rate);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRate(int id)
    {
        var rate = await _rateRepository.GetByIdAsync(id);
        if (rate == null)
        {
            return NotFound();
        }

        return View(rate);
    }

    [HttpPost, ActionName("DeleteRate")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRateConfirmed(int id)
    {
        var rate = await _rateRepository.GetByIdAsync(id);
        if (rate != null)
        {
            rate.ModifiedBy = User.Identity?.Name ?? "Unknown";
            await _rateRepository.DeleteAsync(rate);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(ShippingRate),
                id,
                AuditAction.Delete);
        }

        return RedirectToAction(nameof(ShippingRates));
    }

    #endregion

    #region Helper Methods

    private async Task PopulateLocationDropdownsAsync()
    {
        var locations = await _locationRepository.ListAllAsync();
        ViewBag.Locations = locations.Where(l => l.IsActive)
                                   .Select(l => new SelectListItem
                                   {
                                       Value = l.Id.ToString(),
                                       Text = $"{l.Name} ({l.Code})"
                                   })
                                   .ToList();
    }

    #endregion
}