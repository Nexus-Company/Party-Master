using Nexus.Party.Master.Api.Models;
using Nexus.Stock.Domain.Helpers;
using System.Net;

namespace Nexus.Party.Master.Api.Controllers;

[Route("api/Accounts"), RequireAuthentication]
public class AccountsController : BaseController
{
    public AccountsController(IConfiguration config, IAuthenticationContextFactory auth) 
        : base(config, auth: auth)
    {
    }

    [HttpGet("Me")]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(AccountResult), (int)HttpStatusCode.OK)]
    public IActionResult Me()
        => Ok(new AccountResult(User!));
}
