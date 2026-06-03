namespace CustomerAccountAPI.Infrastructure.Repositories;

using CustomerAccountAPI.Domain.Entities;
using CustomerAccountAPI.Domain.Interfaces;
using CustomerAccountAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public sealed class AccountRepository : IAccountRepository
{
    private readonly AppDbContext _db;

    public AccountRepository(AppDbContext db) => _db = db;

    public async Task<Account?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.Accounts.FindAsync([id], ct);

    public async Task<Account?> GetByIdForCustomerAsync(int id, string customerId, CancellationToken ct = default)
        => await _db.Accounts.FirstOrDefaultAsync(a => a.Id == id && a.CustomerId == customerId, ct);

    public async Task<IReadOnlyList<Account>> GetByCustomerIdAsync(string customerId, CancellationToken ct = default)
        => await _db.Accounts.Where(a => a.CustomerId == customerId).ToListAsync(ct);

    public async Task<IReadOnlyList<Account>> GetAllAsync(CancellationToken ct = default)
        => await _db.Accounts.Include(a => a.Customer).ToListAsync(ct);

    public async Task AddAsync(Account account, CancellationToken ct = default)
        => await _db.Accounts.AddAsync(account, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);
}
