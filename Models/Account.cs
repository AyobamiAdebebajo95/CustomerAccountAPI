// Models/Account.cs
using CustomerAccountAPI.Models;

public class Account
{
    public int Id { get; set; }
    public string AccountNumber { get; set; }
    public decimal Balance { get; set; }
    public string AccountType { get; set; } // Savings, Checking, etc.
    public DateTime OpenedDate { get; set; }
    public bool IsActive { get; set; }

    // Foreign key to Customer
    public string CustomerId { get; set; }
    public virtual Customer Customer { get; set; }
}