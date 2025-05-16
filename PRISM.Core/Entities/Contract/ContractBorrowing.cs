namespace PRISM.Core.Entities.Contract;

/// <summary>
/// Represents UF6 borrowing activity for a contract
/// </summary>
public class ContractBorrowing : BaseEntity
{
    public int ContractId { get; set; }
    public DateTime BorrowDate { get; set; }
    public decimal BorrowedQuantity { get; set; }
    public DateTime ScheduledRepaymentDate { get; set; }
    public DateTime? ActualRepaymentDate { get; set; }
    public decimal? RepaidQuantity { get; set; }
    public string Notes { get; set; } = string.Empty;
    public bool IsRepaid { get; set; } = false;

    // Navigation property
    public virtual Contract Contract { get; set; } = null!;
}