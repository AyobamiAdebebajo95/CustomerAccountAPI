namespace CustomerAccountAPI.Application.Features;

using CustomerAccountAPI.Domain.Common;
using CustomerAccountAPI.Domain.Interfaces;
using FluentValidation;
using MediatR;

// ── Command ──
public sealed record TransferMoneyCommand(
    int FromAccountId,
    int ToAccountId,
    decimal Amount,
    string CustomerId) : IRequest<Result<string>>;

// ── Validator ──
public sealed class TransferMoneyValidator : AbstractValidator<TransferMoneyCommand>
{
    public TransferMoneyValidator()
    {
        RuleFor(x => x.FromAccountId).GreaterThan(0).WithMessage("Source account ID is required.");
        RuleFor(x => x.ToAccountId).GreaterThan(0).WithMessage("Destination account ID is required.");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Transfer amount must be positive.");
        RuleFor(x => x.CustomerId).NotEmpty().WithMessage("Customer ID is required.");
        RuleFor(x => x).Must(x => x.FromAccountId != x.ToAccountId)
            .WithMessage("Cannot transfer to the same account.");
    }
}

// ── Handler ──
public sealed class TransferMoneyHandler : IRequestHandler<TransferMoneyCommand, Result<string>>
{
    private readonly IAccountRepository _repo;

    public TransferMoneyHandler(IAccountRepository repo) => _repo = repo;

    public async Task<Result<string>> Handle(TransferMoneyCommand request, CancellationToken ct)
    {
        var from = await _repo.GetByIdForCustomerAsync(request.FromAccountId, request.CustomerId, ct);
        if (from is null)
            return Result<string>.Failure("Source account not found.");

        var to = await _repo.GetByIdAsync(request.ToAccountId, ct);
        if (to is null)
            return Result<string>.Failure("Destination account not found.");

        if (from.Balance < request.Amount)
            return Result<string>.Failure("Insufficient funds.");

        from.Debit(request.Amount);
        to.Credit(request.Amount);

        await _repo.SaveChangesAsync(ct);
        return Result<string>.Success("Transfer completed successfully.");
    }
}
