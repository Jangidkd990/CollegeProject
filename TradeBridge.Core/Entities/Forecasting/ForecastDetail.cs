using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradeBridge.Core.Entities.Forecasting
{
    public class ForecastDetail : BaseEntity
    {
        [Required]
        public int ForecastId { get; set; }

        [ForeignKey("ForecastId")]
        public Forecast? Forecast { get; set; }

        [Required]
        public DateTime PeriodStart { get; set; }

        [Required]
        public DateTime PeriodEnd { get; set; }

        public int? ContractId { get; set; }

        public int? CustomerId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ForecastValue { get; set; }

        public string Notes { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ActualValue { get; set; }

        public DateTime? ActualDate { get; set; }

        [NotMapped]
        public decimal Variance => ActualValue.HasValue ? ActualValue.Value - ForecastValue : 0;
    }
}