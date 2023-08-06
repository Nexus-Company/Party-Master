using Microsoft.AspNetCore.Mvc;

namespace Nexus.Party.Master.Api.Controllers;

[Route("api/Accounts"), RequireAuthentication]
public class AccountsController : BaseController
{
    public AccountsController(IConfiguration config) : base(config)
    {
    }

    [HttpGet("Me")]
    public IActionResult Me()
    {
        return Ok(User);
    }
}
