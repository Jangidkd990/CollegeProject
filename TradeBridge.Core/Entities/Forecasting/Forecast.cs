using System.ComponentModel.DataAnnotations;

namespace TradeBridge.Core.Entities.Forecasting
{
    public class Forecast : BaseEntity
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DateTime ForecastDate { get; set; }

        [Required]
        public ForecastPeriodType PeriodType { get; set; }

        [Required]
        public ForecastType Type { get; set; }

        public string Description { get; set; } = string.Empty;

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public bool IsApproved { get; set; } = false;

        public string ApprovedBy { get; set; } = string.Empty;

        public DateTime? ApprovalDate { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        public new string CreatedBy { get; set; } = string.Empty;

        public DateTime? LastModifiedDate { get; set; }

        public string LastModifiedBy { get; set; } = string.Empty;
    }

    public enum ForecastPeriodType
    {
        Monthly,
        Quarterly,
        Yearly
    }

    public enum ForecastType
    {
        Quantity,
        Price,
        Revenue,
        UF6Borrowing,
        UF6Repayment
    }
}