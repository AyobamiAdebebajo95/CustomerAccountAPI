namespace CustomerAccountAPI.Application.Features;

using CustomerAccountAPI.Application.Contracts;
using CustomerAccountAPI.Domain.Common;
using CustomerAccountAPI.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// ══════════════════════════════════════════
// Get All Customers
// ══════════════════════════════════════════
public sealed record GetAllCustomersQuery : IRequest<IReadOnlyList<CustomerResponse>>;

public sealed class GetAllCustomersHandler : IRequestHandler<GetAllCustomersQuery, IReadOnlyList<CustomerResponse>>
{
    private readonly UserManager<Customer> _userManager;

    public GetAllCustomersHandler(UserManager<Customer> userManager) => _userManager = userManager;

    public async Task<IReadOnlyList<CustomerResponse>> Handle(GetAllCustomersQuery request, CancellationToken ct)
    {
        var customers = await _userManager.Users
            .Include(c => c.Accounts)
            .ToListAsync(ct);

        return customers.Select(c => new CustomerResponse(
            c.Id, c.Email!, c.FirstName, c.LastName, c.DateOfBirth, c.PhoneNumber, c.CreatedAt,
            c.Accounts.Select(a => new AccountResponse(
                a.Id, a.AccountNumber, a.Balance, a.AccountType, a.OpenedDate, a.IsActive)).ToList()
        )).ToList();
    }
}

// ══════════════════════════════════════════
// Get Customer By ID
// ══════════════════════════════════════════
public sealed record GetCustomerByIdQuery(string CustomerId) : IRequest<Result<CustomerResponse>>;

public sealed class GetCustomerByIdHandler : IRequestHandler<GetCustomerByIdQuery, Result<CustomerResponse>>
{
    private readonly UserManager<Customer> _userManager;

    public GetCustomerByIdHandler(UserManager<Customer> userManager) => _userManager = userManager;

    public async Task<Result<CustomerResponse>> Handle(GetCustomerByIdQuery request, CancellationToken ct)
    {
        var customer = await _userManager.Users
            .Include(c => c.Accounts)
            .FirstOrDefaultAsync(c => c.Id == request.CustomerId, ct);

        if (customer is null)
            return Result<CustomerResponse>.Failure("Customer not found.");

        return Result<CustomerResponse>.Success(new CustomerResponse(
            customer.Id, customer.Email!, customer.FirstName, customer.LastName,
            customer.DateOfBirth, customer.PhoneNumber, customer.CreatedAt,
            customer.Accounts.Select(a => new AccountResponse(
                a.Id, a.AccountNumber, a.Balance, a.AccountType, a.OpenedDate, a.IsActive)).ToList()));
    }
}

// ══════════════════════════════════════════
// Delete Customer
// ══════════════════════════════════════════
public sealed record DeleteCustomerCommand(string CustomerId) : IRequest<Result<string>>;

public sealed class DeleteCustomerHandler : IRequestHandler<DeleteCustomerCommand, Result<string>>
{
    private readonly UserManager<Customer> _userManager;

    public DeleteCustomerHandler(UserManager<Customer> userManager) => _userManager = userManager;

    public async Task<Result<string>> Handle(DeleteCustomerCommand request, CancellationToken ct)
    {
        var customer = await _userManager.FindByIdAsync(request.CustomerId);
        if (customer is null)
            return Result<string>.Failure("Customer not found.");

        var result = await _userManager.DeleteAsync(customer);
        if (!result.Succeeded)
            return Result<string>.Failure(string.Join("; ", result.Errors.Select(e => e.Description)));

        return Result<string>.Success("Customer deleted successfully.");
    }
}

// ══════════════════════════════════════════
// Assign Role
// ══════════════════════════════════════════
public sealed record AssignRoleCommand(string CustomerId, string Role) : IRequest<Result<string>>;

public sealed class AssignRoleHandler : IRequestHandler<AssignRoleCommand, Result<string>>
{
    private readonly UserManager<Customer> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AssignRoleHandler(UserManager<Customer> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Result<string>> Handle(AssignRoleCommand request, CancellationToken ct)
    {
        var customer = await _userManager.FindByIdAsync(request.CustomerId);
        if (customer is null)
            return Result<string>.Failure("Customer not found.");

        if (!await _roleManager.RoleExistsAsync(request.Role))
            return Result<string>.Failure($"Role '{request.Role}' does not exist.");

        if (await _userManager.IsInRoleAsync(customer, request.Role))
            return Result<string>.Failure($"Customer already has the '{request.Role}' role.");

        var result = await _userManager.AddToRoleAsync(customer, request.Role);
        if (!result.Succeeded)
            return Result<string>.Failure(string.Join("; ", result.Errors.Select(e => e.Description)));

        return Result<string>.Success($"Role '{request.Role}' assigned successfully.");
    }
}
