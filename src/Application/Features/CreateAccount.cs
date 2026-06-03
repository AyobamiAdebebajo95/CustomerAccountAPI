namespace CustomerAccountAPI.Application.Features;

using CustomerAccountAPI.Application.Contracts;
using CustomerAccountAPI.Domain.Common;
using CustomerAccountAPI.Domain.Entities;
using CustomerAccountAPI.Domain.Interfaces;
using FluentValidation;
using MediatR;

// ── Command ──
public sealed record CreateAccountCommand(
    string AccountType,
    decimal InitialDeposit,
    string CustomerId) : IRequest<Result<AccountResponse>>;

// ── Validator ──
public sealed class CreateAccountValidator : AbstractValidator<CreateAccountCommand>
{
    private static readonly string[] ValidTypes = ["Savings", "Checking", "Business"];

    public CreateAccountValidator()
    {
        RuleFor(x => x.AccountType)
            .NotEmpty().WithMessage("Account type is required.")
            .Must(t => ValidTypes.Contains(t, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Account type must be one of: {string.Join(", ", ValidTypes)}.");

        RuleFor(x => x.InitialDeposit)
            .GreaterThanOrEqualTo(0).WithMessage("Initial deposit cannot be negative.");

        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required.");
    }
}

// ── Handler ──
public sealed class CreateAccountHandler : IRequestHandler<CreateAccountCommand, Result<AccountResponse>>
{
    private readonly IAccountRepository _repo;

    public CreateAccountHandler(IAccountRepository repo) => _repo = repo;

    public async Task<Result<AccountResponse>> Handle(CreateAccountCommand request, CancellationToken ct)
    {
        var account = new Account
        {
            AccountNumber = $"ACC{DateTime.UtcNow:yyyyMMdd}{Random.Shared.Next(1000, 9999)}",
            Balance = request.InitialDeposit,
            AccountType = request.AccountType,
            OpenedDate = DateTime.UtcNow,
            IsActive = true,
            CustomerId = request.CustomerId
        };

        await _repo.AddAsync(account, ct);
        await _repo.SaveChangesAsync(ct);

        return Result<AccountResponse>.Success(new AccountResponse(
            account.Id, account.AccountNumber, account.Balance,
            account.AccountType, account.OpenedDate, account.IsActive));
    }
}
