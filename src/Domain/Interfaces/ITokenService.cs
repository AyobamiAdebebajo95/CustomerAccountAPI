namespace CustomerAccountAPI.Domain.Interfaces;

using CustomerAccountAPI.Domain.Entities;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(Customer user);
}
