namespace CustomerAccountAPI.Domain.Entities;

public class Account
{
    public int Id { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string AccountType { get; set; } = string.Empty;
    public DateTime OpenedDate { get; set; }
    public bool IsActive { get; set; }
    public string CustomerId { get; set; } = string.Empty;

    public virtual Customer Customer { get; set; } = null!;

    public void Debit(decimal amount)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Debit amount must be positive.");
        if (Balance < amount)
            throw new InvalidOperationException("Insufficient funds.");
        Balance -= amount;
    }

    public void Credit(decimal amount)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Credit amount must be positive.");
        Balance += amount;
    }
}
