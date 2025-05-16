namespace PRISM.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PRISM.Core.Entities.Contract;
using PRISM.Core.Entities.Customer;
using PRISM.Core.Entities.Location;
using PRISM.Core.Entities.Supplier;
using PRISM.Core.Enums;
using PRISM.Core.Interfaces;
using System.Threading.Tasks;

[Authorize]
public class ContractsController : Controller
{
    private readonly IRepository<Contract> _contractRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<Supplier> _supplierRepository;
    private readonly IRepository<ContractDelivery> _deliveryRepository;
    private readonly IRepository<ContractBorrowing> _borrowingRepository;
    private readonly IRepository<Location> _locationRepository;
    private readonly IAuditService _auditService;

    public ContractsController(
        IRepository<Contract> contractRepository,
        IRepository<Customer> customerRepository,
        IRepository<Supplier> supplierRepository,
        IRepository<ContractDelivery> deliveryRepository,
        IRepository<ContractBorrowing> borrowingRepository,
        IRepository<Location> locationRepository,
        IAuditService auditService)
    {
        _contractRepository = contractRepository;
        _customerRepository = customerRepository;
        _supplierRepository = supplierRepository;
        _deliveryRepository = deliveryRepository;
        _borrowingRepository = borrowingRepository;
        _locationRepository = locationRepository;
        _auditService = auditService;
    }

    public async Task<IActionResult> Index()
    {
        var contracts = await _contractRepository.ListAllAsync();
        return View(contracts);
    }

    public async Task<IActionResult> Details(int id)
    {
        var contract = await _contractRepository.GetByIdAsync(id);
        if (contract == null)
        {
            return NotFound();
        }

        await _auditService.LogAuditAsync(
            User.Identity?.Name ?? "Unknown",
            nameof(Contract),
            id,
            AuditAction.Read);

        return View(contract);
    }

    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create()
    {
        await PopulateDropDownLists();
        return View(new Contract
        {
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddYears(1),
            IsActive = true
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create(Contract contract)
    {
        if (ModelState.IsValid)
        {
            contract.CreatedBy = User.Identity?.Name ?? "Unknown";
            var result = await _contractRepository.AddAsync(contract);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(Contract),
                result.Id,
                AuditAction.Create);

            return RedirectToAction(nameof(Details), new { id = result.Id });
        }

        await PopulateDropDownLists(contract);
        return View(contract);
    }

    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id)
    {
        var contract = await _contractRepository.GetByIdAsync(id);
        if (contract == null)
        {
            return NotFound();
        }

        await PopulateDropDownLists(contract);
        return View(contract);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id, Contract contract)
    {
        if (id != contract.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            contract.ModifiedBy = User.Identity?.Name ?? "Unknown";
            contract.ModifiedAt = DateTime.UtcNow;
            await _contractRepository.UpdateAsync(contract);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(Contract),
                contract.Id,
                AuditAction.Update);

            return RedirectToAction(nameof(Details), new { id = contract.Id });
        }

        await PopulateDropDownLists(contract);
        return View(contract);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var contract = await _contractRepository.GetByIdAsync(id);
        if (contract == null)
        {
            return NotFound();
        }

        return View(contract);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var contract = await _contractRepository.GetByIdAsync(id);
        if (contract != null)
        {
            contract.ModifiedBy = User.Identity?.Name ?? "Unknown";
            await _contractRepository.DeleteAsync(contract);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(Contract),
                id,
                AuditAction.Delete);
        }

        return RedirectToAction(nameof(Index));
    }

    // Delivery Management
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AddDelivery(int contractId)
    {
        var contract = await _contractRepository.GetByIdAsync(contractId);
        if (contract == null)
        {
            return NotFound();
        }

        ViewBag.ContractId = contractId;
        ViewBag.ContractNumber = contract.ContractNumber;
        await PopulateLocationDropDownList();

        return View(new ContractDelivery
        {
            ContractId = contractId,
            ScheduledDate = DateTime.Today.AddMonths(1)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AddDelivery(ContractDelivery delivery)
    {
        if (ModelState.IsValid)
        {
            delivery.CreatedBy = User.Identity?.Name ?? "Unknown";
            await _deliveryRepository.AddAsync(delivery);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(ContractDelivery),
                delivery.Id,
                AuditAction.Create);

            return RedirectToAction(nameof(Details), new { id = delivery.ContractId });
        }

        var contract = await _contractRepository.GetByIdAsync(delivery.ContractId);
        ViewBag.ContractId = delivery.ContractId;
        ViewBag.ContractNumber = contract?.ContractNumber ?? "Unknown Contract";

        return View(delivery);
    }

    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> EditDelivery(int id)
    {
        var delivery = await _deliveryRepository.GetByIdAsync(id);
        if (delivery == null)
        {
            return NotFound();
        }

        var contract = await _contractRepository.GetByIdAsync(delivery.ContractId);
        ViewBag.ContractId = delivery.ContractId;
        ViewBag.ContractNumber = contract?.ContractNumber ?? "Unknown Contract";
        await PopulateLocationDropDownList(delivery.LocationId);

        return View(delivery);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> EditDelivery(int id, ContractDelivery delivery)
    {
        if (id != delivery.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            delivery.ModifiedBy = User.Identity?.Name ?? "Unknown";
            delivery.ModifiedAt = DateTime.UtcNow;
            await _deliveryRepository.UpdateAsync(delivery);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(ContractDelivery),
                delivery.Id,
                AuditAction.Update);

            return RedirectToAction(nameof(Details), new { id = delivery.ContractId });
        }

        var contract = await _contractRepository.GetByIdAsync(delivery.ContractId);
        ViewBag.ContractId = delivery.ContractId;
        ViewBag.ContractNumber = contract?.ContractNumber ?? "Unknown Contract";

        return View(delivery);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteDelivery(int id)
    {
        var delivery = await _deliveryRepository.GetByIdAsync(id);
        if (delivery == null)
        {
            return NotFound();
        }

        var contractId = delivery.ContractId;

        delivery.ModifiedBy = User.Identity?.Name ?? "Unknown";
        await _deliveryRepository.DeleteAsync(delivery);

        await _auditService.LogAuditAsync(
            User.Identity?.Name ?? "Unknown",
            nameof(ContractDelivery),
            id,
            AuditAction.Delete);

        return RedirectToAction(nameof(Details), new { id = contractId });
    }

    // Borrowing Management
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AddBorrowing(int contractId)
    {
        var contract = await _contractRepository.GetByIdAsync(contractId);
        if (contract == null)
        {
            return NotFound();
        }

        ViewBag.ContractId = contractId;
        ViewBag.ContractNumber = contract.ContractNumber;

        return View(new ContractBorrowing
        {
            ContractId = contractId,
            BorrowDate = DateTime.Today,
            ScheduledRepaymentDate = DateTime.Today.AddMonths(3)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AddBorrowing(ContractBorrowing borrowing)
    {
        if (ModelState.IsValid)
        {
            borrowing.CreatedBy = User.Identity?.Name ?? "Unknown";
            await _borrowingRepository.AddAsync(borrowing);

            await _auditService.LogAuditAsync(
                User.Identity?.Name ?? "Unknown",
                nameof(ContractBorrowing),
                borrowing.Id,
                AuditAction.Create);

            return RedirectToAction(nameof(Details), new { id = borrowing.ContractId });
        }

        var contract = await _contractRepository.GetByIdAsync(borrowing.ContractId);
        ViewBag.ContractId = borrowing.ContractId;
        ViewBag.ContractNumber = contract?.ContractNumber ?? "Unknown Contract";

        return View(borrowing);
    }

    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> EditBorrowing(int id)
    {
        var borrowing = await _borrowingRepository.GetByIdAsync(id);
        if (borrowing == null)
        {
            return NotFound();
        }

        var contract = await _contractRepository.GetByIdAsync(borrowing.ContractId);
        ViewBag.ContractId = borrowing.ContractId;
        ViewBag.ContractNumber = contract?.ContractNumber ?? "Unknown Contract";

        return View(borrowing);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> EditBorrowing(int id, ContractBorrowing borrowing)
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
                nameof(ContractBorrowing),
                borrowing.Id,
                AuditAction.Update);

            return RedirectToAction(nameof(Details), new { id = borrowing.ContractId });
        }

        var contract = await _contractRepository.GetByIdAsync(borrowing.ContractId);
        ViewBag.ContractId = borrowing.ContractId;
        ViewBag.ContractNumber = contract?.ContractNumber ?? "Unknown Contract";

        return View(borrowing);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteBorrowing(int id)
    {
        var borrowing = await _borrowingRepository.GetByIdAsync(id);
        if (borrowing == null)
        {
            return NotFound();
        }

        var contractId = borrowing.ContractId;

        borrowing.ModifiedBy = User.Identity?.Name ?? "Unknown";
        await _borrowingRepository.DeleteAsync(borrowing);

        await _auditService.LogAuditAsync(
            User.Identity?.Name ?? "Unknown",
            nameof(ContractBorrowing),
            id,
            AuditAction.Delete);

        return RedirectToAction(nameof(Details), new { id = contractId });
    }

    // Helper Methods
    private async Task PopulateDropDownLists(Contract? contract = null)
    {
        var customers = await _customerRepository.ListAsync(c => c.IsActive);
        var suppliers = await _supplierRepository.ListAsync(s => s.IsActive);

        ViewBag.Customers = new SelectList(customers, "Id", "Name", contract?.CustomerId);
        ViewBag.Suppliers = new SelectList(suppliers, "Id", "Name", contract?.SupplierId);
        ViewBag.ContractTypes = new SelectList(Enum.GetValues(typeof(ContractType))
            .Cast<ContractType>()
            .Select(t => new { Id = (int)t, Name = t.ToString() }),
            "Id", "Name", contract != null ? (int)contract.ContractType : null);
        ViewBag.PricingTypes = new SelectList(Enum.GetValues(typeof(PricingType))
            .Cast<PricingType>()
            .Select(t => new { Id = (int)t, Name = t.ToString() }),
            "Id", "Name", contract != null ? (int)contract.PricingType : null);
    }

    private async Task PopulateLocationDropDownList(int? selectedLocationId = null)
    {
        var locations = await _locationRepository.ListAsync(l => l.IsActive);
        ViewBag.Locations = new SelectList(locations, "Id", "Name", selectedLocationId);
    }
}