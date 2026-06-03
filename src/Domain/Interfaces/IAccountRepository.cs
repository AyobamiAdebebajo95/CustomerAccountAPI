namespace CustomerAccountAPI.Domain.Interfaces;

using CustomerAccountAPI.Domain.Entities;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Account?> GetByIdForCustomerAsync(int id, string customerId, CancellationToken ct = default);
    Task<IReadOnlyList<Account>> GetByCustomerIdAsync(string customerId, CancellationToken ct = default);
    Task<IReadOnlyList<Account>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Account account, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
