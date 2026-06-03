namespace CustomerAccountAPI.Application.Features;

using CustomerAccountAPI.Application.Contracts;
using CustomerAccountAPI.Domain.Common;
using CustomerAccountAPI.Domain.Interfaces;
using MediatR;

// ── Get My Accounts ──
public sealed record GetMyAccountsQuery(string CustomerId) : IRequest<IReadOnlyList<AccountResponse>>;

public sealed class GetMyAccountsHandler : IRequestHandler<GetMyAccountsQuery, IReadOnlyList<AccountResponse>>
{
    private readonly IAccountRepository _repo;

    public GetMyAccountsHandler(IAccountRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<AccountResponse>> Handle(GetMyAccountsQuery request, CancellationToken ct)
    {
        var accounts = await _repo.GetByCustomerIdAsync(request.CustomerId, ct);
        return accounts.Select(MapToResponse).ToList();
    }

    private static AccountResponse MapToResponse(Domain.Entities.Account a) =>
        new(a.Id, a.AccountNumber, a.Balance, a.AccountType, a.OpenedDate, a.IsActive);
}

// ── Get Account By ID ──
public sealed record GetAccountByIdQuery(int AccountId, string CustomerId) : IRequest<Result<AccountResponse>>;

public sealed class GetAccountByIdHandler : IRequestHandler<GetAccountByIdQuery, Result<AccountResponse>>
{
    private readonly IAccountRepository _repo;

    public GetAccountByIdHandler(IAccountRepository repo) => _repo = repo;

    public async Task<Result<AccountResponse>> Handle(GetAccountByIdQuery request, CancellationToken ct)
    {
        var account = await _repo.GetByIdForCustomerAsync(request.AccountId, request.CustomerId, ct);

        if (account is null)
            return Result<AccountResponse>.Failure("Account not found.");

        return Result<AccountResponse>.Success(
            new(account.Id, account.AccountNumber, account.Balance,
                account.AccountType, account.OpenedDate, account.IsActive));
    }
}

// ── Get All Accounts (Admin) ──
public sealed record GetAllAccountsQuery : IRequest<IReadOnlyList<AccountResponse>>;

public sealed class GetAllAccountsHandler : IRequestHandler<GetAllAccountsQuery, IReadOnlyList<AccountResponse>>
{
    private readonly IAccountRepository _repo;

    public GetAllAccountsHandler(IAccountRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<AccountResponse>> Handle(GetAllAccountsQuery request, CancellationToken ct)
    {
        var accounts = await _repo.GetAllAsync(ct);
        return accounts.Select(a =>
            new AccountResponse(a.Id, a.AccountNumber, a.Balance, a.AccountType, a.OpenedDate, a.IsActive))
            .ToList();
    }
}
