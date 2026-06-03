namespace CustomerAccountAPI.Domain.Entities;

using Microsoft.AspNetCore.Identity;

public class Customer : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = [];
}
