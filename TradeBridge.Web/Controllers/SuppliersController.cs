namespace TradeBridge.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeBridge.Core.Entities.Supplier;
using TradeBridge.Core.Interfaces;
using System.Threading.Tasks;

[Authorize]
public class SuppliersController : Controller
{
    private readonly IRepository<Supplier> _supplierRepository;
    private readonly IRepository<SupplierContact> _contactRepository;
    private readonly IAuditService _auditService;

    public SuppliersController(
        IRepository<Supplier> supplierRepository,
        IRepository<SupplierContact> contactRepository,
        IAuditService auditService)
    {
        _supplierRepository = supplierRepository;
        _contactRepository = contactRepository;
        _auditService = auditService;
    }

    public async Task<IActionResult> Index()
    {
        var suppliers = await _supplierRepository.ListAllAsync();
        return View(suppliers);
    }

    public async Task<IActionResult> Details(int id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null)
        {
            return NotFound();
        }

        await _auditService.LogAuditAsync(
            User.Identity?.Name ?? "Unknown",
            nameof(Supplier),
            id,
            AuditAction.Read);

        return View(supplier);
    }

    [Authorize(Roles = "Admin,Manager")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create(Supplier supplier)
    {
        if (ModelState.IsValid)
        {
            supplier.CreatedBy = User.Identity?.Name ?? "Unknown";
            var result = await _supplierRepository.AddAsync(supplier);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(Supplier),
                result.Id,
                AuditAction.Create);

            return RedirectToAction(nameof(Index));
        }

        return View(supplier);
    }

    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null)
        {
            return NotFound();
        }

        return View(supplier);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id, Supplier supplier)
    {
        if (id != supplier.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            supplier.ModifiedBy = User.Identity?.Name ?? "Unknown";
            await _supplierRepository.UpdateAsync(supplier);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(Supplier),
                supplier.Id,
                AuditAction.Update);

            return RedirectToAction(nameof(Index));
        }

        return View(supplier);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null)
        {
            return NotFound();
        }

        return View(supplier);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier != null)
        {
            supplier.ModifiedBy = User.Identity?.Name ?? "Unknown";
            await _supplierRepository.DeleteAsync(supplier);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(Supplier),
                id,
                AuditAction.Delete);
        }

        return RedirectToAction(nameof(Index));
    }

    // Contact Management
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AddContact(int supplierId)
    {
        var supplier = await _supplierRepository.GetByIdAsync(supplierId);
        if (supplier == null)
        {
            return NotFound();
        }

        ViewBag.SupplierId = supplierId;
        ViewBag.SupplierName = supplier.Name;

        return View(new SupplierContact { SupplierId = supplierId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AddContact(SupplierContact contact)
    {
        if (ModelState.IsValid)
        {
            contact.CreatedBy = User.Identity?.Name ?? "Unknown";
            await _contactRepository.AddAsync(contact);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(SupplierContact),
                contact.Id,
                AuditAction.Create);

            return RedirectToAction(nameof(Details), new { id = contact.SupplierId });
        }

        var supplier = await _supplierRepository.GetByIdAsync(contact.SupplierId);
        ViewBag.SupplierId = contact.SupplierId;
        ViewBag.SupplierName = supplier?.Name ?? "Unknown Supplier";

        return View(contact);
    }

    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> EditContact(int id)
    {
        var contact = await _contactRepository.GetByIdAsync(id);
        if (contact == null)
        {
            return NotFound();
        }

        var supplier = await _supplierRepository.GetByIdAsync(contact.SupplierId);
        ViewBag.SupplierId = contact.SupplierId;
        ViewBag.SupplierName = supplier?.Name ?? "Unknown Supplier";

        return View(contact);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> EditContact(int id, SupplierContact contact)
    {
        if (id != contact.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            contact.ModifiedBy = User.Identity?.Name ?? "Unknown";
            await _contactRepository.UpdateAsync(contact);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(SupplierContact),
                contact.Id,
                AuditAction.Update);

            return RedirectToAction(nameof(Details), new { id = contact.SupplierId });
        }

        var supplier = await _supplierRepository.GetByIdAsync(contact.SupplierId);
        ViewBag.SupplierId = contact.SupplierId;
        ViewBag.SupplierName = supplier?.Name ?? "Unknown Supplier";

        return View(contact);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteContact(int id)
    {
        var contact = await _contactRepository.GetByIdAsync(id);
        if (contact == null)
        {
            return NotFound();
        }

        var supplierId = contact.SupplierId;

        contact.ModifiedBy = User.Identity?.Name ?? "Unknown";
        await _contactRepository.DeleteAsync(contact);

        await _auditService.LogAuditAsync(
            User.Identity?.Name ?? "Unknown",
            nameof(SupplierContact),
            id,
            AuditAction.Delete);

        return RedirectToAction(nameof(Details), new { id = supplierId });
    }
}