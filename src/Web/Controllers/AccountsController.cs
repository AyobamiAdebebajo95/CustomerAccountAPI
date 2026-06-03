namespace CustomerAccountAPI.Web.Controllers;

using System.Security.Claims;
using CustomerAccountAPI.Application.Features;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/accounts")]
[Authorize]
public class AccountsController(IMediator mediator) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<IActionResult> GetMyAccounts()
        => Ok(await mediator.Send(new GetMyAccountsQuery(UserId)));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAccount(int id)
    {
        var result = await mediator.Send(new GetAccountByIdQuery(id, UserId));
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { message = result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest req)
    {
        var result = await mediator.Send(new CreateAccountCommand(req.AccountType, req.InitialDeposit, UserId));
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAccount), new { id = result.Value!.Id }, result.Value)
            : BadRequest(new { message = result.Error });
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest req)
    {
        var result = await mediator.Send(new TransferMoneyCommand(req.FromAccountId, req.ToAccountId, req.Amount, UserId));
        return result.IsSuccess
            ? Ok(new { message = result.Value })
            : BadRequest(new { message = result.Error });
    }
}

public sealed record CreateAccountRequest(string AccountType, decimal InitialDeposit);
public sealed record TransferRequest(int FromAccountId, int ToAccountId, decimal Amount);
