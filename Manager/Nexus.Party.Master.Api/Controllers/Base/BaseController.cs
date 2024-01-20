using Microsoft.AspNetCore.Cors;
using Nexus.Party.Master.Dal;
using Nexus.Party.Master.Domain.Models;
using Nexus.Stock.Domain.Helpers;
using System.Net;
using Account = Nexus.Party.Master.Dal.Models.Accounts.Account;

namespace Nexus.Party.Master.Api.Controllers.Base;

[EnableCors]
[RequireHttps]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    private protected readonly AuthenticationContext authCtx;
    private protected readonly InteractContext interContext;
    private protected readonly IAuthenticationContextFactory? auth;
    public readonly Config Config;

    private protected new Account? User 
        => auth?.Account;

    public BaseController(IConfiguration config, AuthenticationContext authCtx = null, IAuthenticationContextFactory? auth = null)
        : base()
    {
        this.authCtx = authCtx;
        interContext = new();
        this.auth = auth;
        var sec = config.GetSection("Config");
        Config = sec.Get<Config>()!;

        if (string.IsNullOrEmpty(Config.WebUrl))
            Config.WebUrl = "https://localhost:44370";
    }

    [NonAction]
    private protected virtual ObjectResult StatusCode(HttpStatusCode statusCode)
        => StatusCode((int)statusCode, null);
}