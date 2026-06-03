namespace CustomerAccountAPI.Application.Features;

using CustomerAccountAPI.Application.Contracts;
using CustomerAccountAPI.Domain.Common;
using CustomerAccountAPI.Domain.Entities;
using CustomerAccountAPI.Domain.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

// ── Command ──
public sealed record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponse>>;

// ── Validator ──
public sealed class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

// ── Handler ──
public sealed class LoginHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly UserManager<Customer> _userManager;
    private readonly SignInManager<Customer> _signInManager;
    private readonly ITokenService _tokenService;

    public LoginHandler(
        UserManager<Customer> userManager,
        SignInManager<Customer> signInManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result<AuthResponse>.Failure("Invalid email or password.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            return Result<AuthResponse>.Failure("Invalid email or password.");

        var token = await _tokenService.GenerateTokenAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        return Result<AuthResponse>.Success(
            new AuthResponse(token, user.Email!, user.Id, roles.ToList()));
    }
}
