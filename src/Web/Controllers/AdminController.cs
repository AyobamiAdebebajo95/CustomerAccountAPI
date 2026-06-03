namespace CustomerAccountAPI.Web.Controllers;

using CustomerAccountAPI.Application.Features;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController(IMediator mediator) : ControllerBase
{
    [HttpGet("customers")]
    public async Task<IActionResult> GetAllCustomers()
        => Ok(await mediator.Send(new GetAllCustomersQuery()));

    [HttpGet("customers/{id}")]
    public async Task<IActionResult> GetCustomer(string id)
    {
        var result = await mediator.Send(new GetCustomerByIdQuery(id));
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { message = result.Error });
    }

    [HttpGet("accounts")]
    public async Task<IActionResult> GetAllAccounts()
        => Ok(await mediator.Send(new GetAllAccountsQuery()));

    [HttpDelete("customers/{id}")]
    public async Task<IActionResult> DeleteCustomer(string id)
    {
        var result = await mediator.Send(new DeleteCustomerCommand(id));
        return result.IsSuccess ? Ok(new { message = result.Value }) : NotFound(new { message = result.Error });
    }

    [HttpPost("customers/{id}/assign-role")]
    public async Task<IActionResult> AssignRole(string id, [FromBody] AssignRoleRequest req)
    {
        var result = await mediator.Send(new AssignRoleCommand(id, req.Role));
        return result.IsSuccess ? Ok(new { message = result.Value }) : BadRequest(new { message = result.Error });
    }
}

public sealed record AssignRoleRequest(string Role);
