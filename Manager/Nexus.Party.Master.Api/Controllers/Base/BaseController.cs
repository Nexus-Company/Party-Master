using Microsoft.AspNetCore.Cors;
using Nexus.Party.Master.Dal;
using System.Net;

namespace Nexus.Party.Master.Api.Controllers.Base;

[EnableCors]
[RequireHttps]
[Route("api/[controller]")]
public class BaseController : ControllerBase
{
    private protected readonly AuthenticationContext authCtx;

    public BaseController()
        : base()
    {
        authCtx = new(@$"Data Source={AppDomain.CurrentDomain.BaseDirectory}\Databases\Authentication.db");
    }


    [NonAction]
    private protected virtual ObjectResult StatusCode(HttpStatusCode statusCode)
        => StatusCode((int)statusCode, null);
}