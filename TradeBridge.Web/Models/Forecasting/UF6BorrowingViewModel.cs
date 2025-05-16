using Microsoft.AspNetCore.Mvc.Rendering;
using TradeBridge.Core.Entities.Forecasting;
using System.ComponentModel.DataAnnotations;

namespace TradeBridge.Web.Models.Forecasting;

public class UF6BorrowingViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Contract is required")]
    [Display(Name = "Contract")]
    public int ContractId { get; set; }

    [Display(Name = "Customer")]
    public int? CustomerId { get; set; }

    [Required(ErrorMessage = "Transaction type is required")]
    [Display(Name = "Transaction Type")]
    public BorrowingTransactionType TransactionType { get; set; }

    [Required(ErrorMessage = "Transaction date is required")]
    [Display(Name = "Transaction Date")]
    [DataType(DataType.Date)]
    public DateTime TransactionDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Quantity is required")]
    [Range(0.01, 99999999.99, ErrorMessage = "Quantity must be positive and less than 99,999,999.99")]
    public decimal Quantity { get; set; }

    [Required(ErrorMessage = "Unit is required")]
    [StringLength(10, ErrorMessage = "Unit cannot exceed 10 characters")]
    public string Unit { get; set; } = "KgU";

    [Required(ErrorMessage = "Due date is required")]
    [Display(Name = "Due Date")]
    [DataType(DataType.Date)]
    public DateTime DueDate { get; set; }

    [Display(Name = "Is Settled")]
    public bool IsSettled { get; set; }

    [Display(Name = "Settlement Date")]
    [DataType(DataType.Date)]
    public DateTime? SettlementDate { get; set; }

    [Display(Name = "Notes")]
    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    [DataType(DataType.MultilineText)]
    public string Notes { get; set; } = string.Empty;

    [Display(Name = "Related Record")]
    public int? RelatedRecordId { get; set; }

    // For display
    public string ContractNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string RelatedTransactionDetails { get; set; } = string.Empty;

    // UI properties
    [Display(Name = "Days Until Due")]
    public int DaysUntilDue => IsSettled ? 0 : (DueDate - DateTime.Today).Days;

    [Display(Name = "Status")]
    public string Status => IsSettled ? "Settled" :
                           (DueDate < DateTime.Today ? "Overdue" :
                           (DueDate < DateTime.Today.AddDays(30) ? "Due Soon" : "Active"));

    // Dropdown options
    public List<SelectListItem> TransactionTypeOptions => Enum.GetValues<BorrowingTransactionType>()
        .Select(t => new SelectListItem
        {
            Value = ((int)t).ToString(),
            Text = t.ToString()
        }).ToList();

    public List<SelectListItem> ContractOptions { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> CustomerOptions { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> RelatedRecordOptions { get; set; } = new List<SelectListItem>();
}