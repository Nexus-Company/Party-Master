using Microsoft.AspNetCore.Cors;
using Nexus.Party.Master.Api.Models;
using Nexus.Party.Master.Dal;
using System.Net;

namespace Nexus.Party.Master.Api.Controllers.Base;

[EnableCors]
[RequireHttps]
[Route("api/[controller]")]
public class BaseController : ControllerBase
{
    private protected readonly AuthenticationContext authCtx;
    public readonly Config Config;

    public static readonly string AuthConnString = @$"Data Source={AppDomain.CurrentDomain.BaseDirectory}\Databases\Authentication.db";
    public BaseController(IConfiguration config)
        : base()
    {
        authCtx = new(AuthConnString);
        var sec = config.GetSection("Config");
        Config = sec.Get<Config>();

        if (string.IsNullOrEmpty(Config.WebUrl))
            Config.WebUrl = "https://localhost:44370";
    }

    [NonAction]
    private protected virtual ObjectResult StatusCode(HttpStatusCode statusCode)
        => StatusCode((int)statusCode, null);
}