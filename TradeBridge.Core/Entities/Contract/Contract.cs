namespace TradeBridge.Core.Entities.Contract;

using TradeBridge.Core.Entities.Customer;
using TradeBridge.Core.Entities.Supplier;
using TradeBridge.Core.Enums;

/// <summary>
/// Represents a contract in the TradeBridge system, which can be either a customer or supplier contract
/// </summary>
public class Contract : BaseEntity
{
    public string ContractNumber { get; set; } = string.Empty;
    public ContractType ContractType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal BaseQuantity { get; set; }
    public decimal? MinQuantity { get; set; }
    public decimal? MaxQuantity { get; set; }
    public PricingType PricingType { get; set; }
    public decimal? FixedPrice { get; set; }
    public decimal? FixedEscalationRate { get; set; }
    public string ContractTerms { get; set; } = string.Empty;
    public string DocumentPath { get; set; } = string.Empty;
    public bool HasDeferralOption { get; set; } = false;
    public bool IsBorrowingAllowed { get; set; } = false;
    public bool IsActive { get; set; } = true;

    // For customer contracts
    public int? CustomerId { get; set; }
    // For supplier contracts
    public int? SupplierId { get; set; }

    // Navigation properties
    public virtual Customer? Customer { get; set; }
    public virtual Supplier? Supplier { get; set; }
    public virtual ICollection<ContractDelivery> Deliveries { get; set; } = new List<ContractDelivery>();
    public virtual ICollection<ContractBorrowing> Borrowings { get; set; } = new List<ContractBorrowing>();
}