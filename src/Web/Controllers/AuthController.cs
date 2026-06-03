namespace CustomerAccountAPI.Web.Controllers;

using CustomerAccountAPI.Application.Features;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var result = await mediator.Send(new RegisterCommand(
            req.Email, req.Password, req.FirstName, req.LastName, req.DateOfBirth, req.PhoneNumber));

        return result.IsSuccess
            ? Ok(new { message = result.Value })
            : BadRequest(new { message = result.Error });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var result = await mediator.Send(new LoginCommand(req.Email, req.Password));

        return result.IsSuccess
            ? Ok(result.Value)
            : Unauthorized(new { message = result.Error });
    }
}

public sealed record RegisterRequest(
    string Email, string Password, string FirstName,
    string LastName, DateTime DateOfBirth, string PhoneNumber);

public sealed record LoginRequest(string Email, string Password);
