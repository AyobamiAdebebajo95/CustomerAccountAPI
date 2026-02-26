// Controllers/AccountsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CustomerAccountAPI.Data;
using CustomerAccountAPI.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Customer> _userManager;

    public AccountsController(ApplicationDbContext context, UserManager<Customer> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccountDto>>> GetMyAccounts()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var accounts = await _context.Accounts
            .Where(a => a.CustomerId == userId)
            .Select(a => new AccountDto
            {
                Id = a.Id,
                AccountNumber = a.AccountNumber,
                Balance = a.Balance,
                AccountType = a.AccountType,
                OpenedDate = a.OpenedDate,
                IsActive = a.IsActive
            })
            .ToListAsync();

        return Ok(accounts);
    }

    [HttpPost]
    public async Task<ActionResult<AccountDto>> CreateAccount([FromBody] CreateAccountDto model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Generate unique account number
        var accountNumber = GenerateAccountNumber();

        var account = new Account
        {
            AccountNumber = accountNumber,
            Balance = model.InitialDeposit,
            AccountType = model.AccountType,
            OpenedDate = DateTime.UtcNow,
            IsActive = true,
            CustomerId = userId
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        var accountDto = new AccountDto
        {
            Id = account.Id,
            AccountNumber = account.AccountNumber,
            Balance = account.Balance,
            AccountType = account.AccountType,
            OpenedDate = account.OpenedDate,
            IsActive = account.IsActive
        };

        return CreatedAtAction(nameof(GetAccount), new { id = account.Id }, accountDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AccountDto>> GetAccount(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == id && a.CustomerId == userId);

        if (account == null)
            return NotFound();

        return Ok(new AccountDto
        {
            Id = account.Id,
            AccountNumber = account.AccountNumber,
            Balance = account.Balance,
            AccountType = account.AccountType,
            OpenedDate = account.OpenedDate,
            IsActive = account.IsActive
        });
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> TransferMoney([FromBody] TransferDto model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Verify ownership of from account
        var fromAccount = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == model.FromAccountId && a.CustomerId == userId);

        if (fromAccount == null)
            return NotFound("Source account not found");

        if (fromAccount.Balance < model.Amount)
            return BadRequest("Insufficient funds");

        // Verify to account exists
        var toAccount = await _context.Accounts
            .FindAsync(model.ToAccountId);

        if (toAccount == null)
            return NotFound("Destination account not found");

        // Perform transfer
        fromAccount.Balance -= model.Amount;
        toAccount.Balance += model.Amount;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Transfer completed successfully" });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin/all")]
    public async Task<ActionResult<IEnumerable<AccountDto>>> GetAllAccounts()
    {
        var accounts = await _context.Accounts
            .Include(a => a.Customer)
            .Select(a => new AccountDto
            {
                Id = a.Id,
                AccountNumber = a.AccountNumber,
                Balance = a.Balance,
                AccountType = a.AccountType,
                OpenedDate = a.OpenedDate,
                IsActive = a.IsActive
            })
            .ToListAsync();

        return Ok(accounts);
    }

    private string GenerateAccountNumber()
    {
        // Simple account number generation - you might want a more sophisticated approach
        var random = new Random();
        return $"ACC{DateTime.Now:yyyyMMdd}{random.Next(1000, 9999)}";
    }
}