using Microsoft.AspNetCore.Mvc.Rendering;
using PRISM.Core.Entities.Forecasting;
using System.ComponentModel.DataAnnotations;

namespace PRISM.Web.Models.Forecasting;

public class ForecastViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Forecast date is required")]
    [Display(Name = "Forecast Date")]
    [DataType(DataType.Date)]
    public DateTime ForecastDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Period type is required")]
    [Display(Name = "Period Type")]
    public ForecastPeriodType PeriodType { get; set; }

    [Required(ErrorMessage = "Forecast type is required")]
    [Display(Name = "Forecast Type")]
    public ForecastType Type { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    [DataType(DataType.MultilineText)]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Is Approved")]
    public bool IsApproved { get; set; }

    [Display(Name = "Approved By")]
    public string ApprovedBy { get; set; } = string.Empty;

    [Display(Name = "Approval Date")]
    [DataType(DataType.Date)]
    public DateTime? ApprovalDate { get; set; }

    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;

    [Display(Name = "Created Date")]
    [DataType(DataType.DateTime)]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Last Modified By")]
    public string LastModifiedBy { get; set; } = string.Empty;

    [Display(Name = "Last Modified Date")]
    [DataType(DataType.DateTime)]
    public DateTime? LastModifiedDate { get; set; }

    public List<ForecastDetailViewModel> Details { get; set; } = new List<ForecastDetailViewModel>();

    // Dropdown options for select lists
    public List<SelectListItem> PeriodTypeOptions => Enum.GetValues<ForecastPeriodType>()
        .Select(p => new SelectListItem
        {
            Value = ((int)p).ToString(),
            Text = p.ToString()
        }).ToList();

    public List<SelectListItem> ForecastTypeOptions => Enum.GetValues<ForecastType>()
        .Select(t => new SelectListItem
        {
            Value = ((int)t).ToString(),
            Text = t.ToString()
        }).ToList();
}