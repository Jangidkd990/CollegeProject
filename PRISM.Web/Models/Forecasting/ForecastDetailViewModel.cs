using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PRISM.Web.Models.Forecasting;

public class ForecastDetailViewModel
{
    public int Id { get; set; }

    public int ForecastId { get; set; }

    [Required(ErrorMessage = "Period start date is required")]
    [Display(Name = "Period Start")]
    [DataType(DataType.Date)]
    public DateTime PeriodStart { get; set; }

    [Required(ErrorMessage = "Period end date is required")]
    [Display(Name = "Period End")]
    [DataType(DataType.Date)]
    public DateTime PeriodEnd { get; set; }

    [Display(Name = "Contract")]
    public int? ContractId { get; set; }

    [Display(Name = "Customer")]
    public int? CustomerId { get; set; }

    [Required(ErrorMessage = "Forecast value is required")]
    [Display(Name = "Forecast Value")]
    [Range(0, 99999999.99, ErrorMessage = "Value must be between 0 and 99,999,999.99")]
    public decimal ForecastValue { get; set; }

    [Display(Name = "Notes")]
    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    [DataType(DataType.MultilineText)]
    public string Notes { get; set; } = string.Empty;

    [Display(Name = "Actual Value")]
    [Range(0, 99999999.99, ErrorMessage = "Value must be between 0 and 99,999,999.99")]
    public decimal? ActualValue { get; set; }

    [Display(Name = "Actual Date")]
    [DataType(DataType.Date)]
    public DateTime? ActualDate { get; set; }

    [Display(Name = "Variance")]
    public decimal Variance => ActualValue.HasValue ? ActualValue.Value - ForecastValue : 0;

    [Display(Name = "Variance %")]
    public decimal VariancePercent => ForecastValue != 0 && ActualValue.HasValue
        ? Math.Round((ActualValue.Value - ForecastValue) / ForecastValue * 100, 2)
        : 0;

    // Navigation properties for display
    public string ContractNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;

    // Dropdown options
    public List<SelectListItem> ContractOptions { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> CustomerOptions { get; set; } = new List<SelectListItem>();
}