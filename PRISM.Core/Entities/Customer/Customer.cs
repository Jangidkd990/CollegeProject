namespace PRISM.Core.Entities.Customer;
/// <summary>
/// Represents a customer in the PRISM system
/// </summary>
public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<CustomerContact> Contacts { get; set; } = new List<CustomerContact>();
    public virtual ICollection<PRISM.Core.Entities.Contract.Contract> Contracts { get; set; } = new List<PRISM.Core.Entities.Contract.Contract>();
}