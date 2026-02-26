// Models/Customer.cs
namespace CustomerAccountAPI.Models;

using Microsoft.AspNetCore.Identity;

public class Customer : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation property
    public virtual ICollection<Account> Accounts { get; set; }
}
