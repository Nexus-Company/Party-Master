using Nexus.Party.Master.Api.Models;
using System.Net;

namespace Nexus.Party.Master.Api.Controllers;

[Route("api/Accounts"), RequireAuthentication]
public class AccountsController : BaseController
{
    public AccountsController(IConfiguration config) : base(config)
    {
    }

    [HttpGet("Me")]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(AccountResult), (int)HttpStatusCode.OK)]
    public IActionResult Me()
        => Ok(new AccountResult(User!));
}
