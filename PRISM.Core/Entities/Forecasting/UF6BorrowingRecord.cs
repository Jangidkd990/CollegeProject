using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRISM.Core.Entities.Forecasting
{
    public class UF6BorrowingRecord : BaseEntity
    {
        [Required]
        public int ContractId { get; set; }

        public int? CustomerId { get; set; }

        [Required]
        public BorrowingTransactionType TransactionType { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        public string Unit { get; set; } = "KgU";

        [Required]
        public DateTime DueDate { get; set; }

        public bool IsSettled { get; set; } = false;

        public DateTime? SettlementDate { get; set; }

        public string Notes { get; set; } = string.Empty;

        public int? RelatedRecordId { get; set; }

        [ForeignKey("RelatedRecordId")]
        public UF6BorrowingRecord? RelatedRecord { get; set; }
    }

    public enum BorrowingTransactionType
    {
        Borrow,
        Repay,
        Defer
    }
}