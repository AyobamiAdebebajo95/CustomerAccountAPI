namespace CustomerAccountAPI.Application.Features;

using CustomerAccountAPI.Domain.Common;
using CustomerAccountAPI.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

// ── Command ──
public sealed record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string PhoneNumber) : IRequest<Result<string>>;

// ── Validator ──
public sealed class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.DateOfBirth).LessThan(DateTime.UtcNow).WithMessage("Date of birth must be in the past.");
        RuleFor(x => x.PhoneNumber).NotEmpty();
    }
}

// ── Handler ──
public sealed class RegisterHandler : IRequestHandler<RegisterCommand, Result<string>>
{
    private readonly UserManager<Customer> _userManager;

    public RegisterHandler(UserManager<Customer> userManager) => _userManager = userManager;

    public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken ct)
    {
        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
            return Result<string>.Failure("A user with this email already exists.");

        var user = new Customer
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth,
            PhoneNumber = request.PhoneNumber,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return Result<string>.Failure(string.Join("; ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, "Customer");
        return Result<string>.Success("User registered successfully.");
    }
}
