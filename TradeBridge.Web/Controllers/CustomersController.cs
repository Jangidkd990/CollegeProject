namespace TradeBridge.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeBridge.Core.Entities.Customer;
using TradeBridge.Core.Interfaces;
using System.Threading.Tasks;

[Authorize]
public class CustomersController : Controller
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IAuditService _auditService;

    public CustomersController(IRepository<Customer> customerRepository, IAuditService auditService)
    {
        _customerRepository = customerRepository;
        _auditService = auditService;
    }

    public async Task<IActionResult> Index()
    {
        var customers = await _customerRepository.ListAllAsync();
        return View(customers);
    }

    public async Task<IActionResult> Details(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
        {
            return NotFound();
        }

        await _auditService.LogAuditAsync(
            User.Identity?.Name ?? "Unknown",
            nameof(Customer),
            id,
            AuditAction.Read);

        return View(customer);
    }

    [Authorize(Roles = "Admin,Manager")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create(Customer customer)
    {
        if (ModelState.IsValid)
        {
            customer.CreatedBy = User.Identity?.Name ?? "Unknown";
            var result = await _customerRepository.AddAsync(customer);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(Customer),
                result.Id,
                AuditAction.Create);

            return RedirectToAction(nameof(Index));
        }

        return View(customer);
    }

    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
        {
            return NotFound();
        }

        return View(customer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id, Customer customer)
    {
        if (id != customer.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            customer.ModifiedBy = User.Identity?.Name ?? "Unknown";
            await _customerRepository.UpdateAsync(customer);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(Customer),
                customer.Id,
                AuditAction.Update);

            return RedirectToAction(nameof(Index));
        }

        return View(customer);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
        {
            return NotFound();
        }

        return View(customer);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer != null)
        {
            customer.ModifiedBy = User.Identity?.Name ?? "Unknown";
            await _customerRepository.DeleteAsync(customer);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(Customer),
                id,
                AuditAction.Delete);
        }

        return RedirectToAction(nameof(Index));
    }
}