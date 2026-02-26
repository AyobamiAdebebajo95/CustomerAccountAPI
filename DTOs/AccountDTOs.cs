// DTOs/AccountDTOs.cs
public class CreateAccountDto
{
    public string AccountType { get; set; }
    public decimal InitialDeposit { get; set; }
}

public class AccountDto
{
    public int Id { get; set; }
    public string AccountNumber { get; set; }
    public decimal Balance { get; set; }
    public string AccountType { get; set; }
    public DateTime OpenedDate { get; set; }
    public bool IsActive { get; set; }
}

public class TransferDto
{
    public int FromAccountId { get; set; }
    public int ToAccountId { get; set; }
    public decimal Amount { get; set; }
}