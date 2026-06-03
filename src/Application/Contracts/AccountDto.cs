namespace CustomerAccountAPI.Application.Contracts;

public sealed record AccountResponse(
    int Id,
    string AccountNumber,
    decimal Balance,
    string AccountType,
    DateTime OpenedDate,
    bool IsActive);

public sealed record CustomerResponse(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string? PhoneNumber,
    DateTime CreatedAt,
    IReadOnlyList<AccountResponse> Accounts);

public sealed record AuthResponse(
    string Token,
    string Email,
    string UserId,
    List<string> Roles);
