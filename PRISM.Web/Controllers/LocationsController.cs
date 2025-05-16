namespace PRISM.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRISM.Core.Entities.Location;
using PRISM.Core.Interfaces;
using System.Threading.Tasks;

[Authorize]
public class LocationsController : Controller
{
    private readonly IRepository<Location> _locationRepository;
    private readonly IAuditService _auditService;

    public LocationsController(
        IRepository<Location> locationRepository,
        IAuditService auditService)
    {
        _locationRepository = locationRepository;
        _auditService = auditService;
    }

    public async Task<IActionResult> Index()
    {
        var locations = await _locationRepository.ListAllAsync();
        return View(locations);
    }

    public async Task<IActionResult> Details(int id)
    {
        var location = await _locationRepository.GetByIdAsync(id);
        if (location == null)
        {
            return NotFound();
        }

        await _auditService.LogAuditAsync(
            User.Identity?.Name ?? "Unknown",
            nameof(Location),
            id,
            AuditAction.Read);

        return View(location);
    }

    [Authorize(Roles = "Admin,Manager")]
    public IActionResult Create()
    {
        return View(new Location { IsActive = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create(Location location)
    {
        if (ModelState.IsValid)
        {
            location.CreatedBy = User.Identity?.Name ?? "Unknown";
            var result = await _locationRepository.AddAsync(location);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(Location),
                result.Id,
                AuditAction.Create);

            return RedirectToAction(nameof(Details), new { id = result.Id });
        }

        return View(location);
    }

    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id)
    {
        var location = await _locationRepository.GetByIdAsync(id);
        if (location == null)
        {
            return NotFound();
        }

        return View(location);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id, Location location)
    {
        if (id != location.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            location.ModifiedBy = User.Identity?.Name ?? "Unknown";
            location.ModifiedAt = DateTime.UtcNow;
            await _locationRepository.UpdateAsync(location);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(Location),
                location.Id,
                AuditAction.Update);

            return RedirectToAction(nameof(Details), new { id = location.Id });
        }

        return View(location);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var location = await _locationRepository.GetByIdAsync(id);
        if (location == null)
        {
            return NotFound();
        }

        return View(location);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var location = await _locationRepository.GetByIdAsync(id);
        if (location != null)
        {
            location.ModifiedBy = User.Identity?.Name ?? "Unknown";
            await _locationRepository.DeleteAsync(location);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(Location),
                id,
                AuditAction.Delete);
        }

        return RedirectToAction(nameof(Index));
    }
}