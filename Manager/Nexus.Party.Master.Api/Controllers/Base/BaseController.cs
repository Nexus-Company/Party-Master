using System.Net;

namespace Nexus.Party.Master.Api.Controllers.Base;

[ApiController]
[Route("api/[controller]")]
public class BaseController : ControllerBase
{
    [NonAction]
    private protected virtual ObjectResult StatusCode(HttpStatusCode statusCode)
        => StatusCode((int)statusCode, null);
}