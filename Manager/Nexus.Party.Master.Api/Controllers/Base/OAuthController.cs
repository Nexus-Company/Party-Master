using Nexus.Party.Master.Domain;
using System.Net;

namespace Nexus.Party.Master.Api.Controllers.Base;

[Route("api/Authentication")]
public partial class OAuthController : BaseController
{
    private protected static string? State { get; set; }
    private protected string ClientId { get; set; }
    private protected string Secret { get; set; }
    private protected string[] Scopes { get; set; }

    public OAuthController(IConfiguration config)
        : base(config)
    {

    }

    private protected OAuthController(IConfiguration config, string configKey)
        : base(config)
    {
        Exception error =
            new ArgumentException("It's not possible start this application without Spotify ClientId, ClientSecret and Scopes.");

        State ??= Guid.NewGuid().ToString();

        ClientId = config[$"{configKey}:ClientId"] ?? throw error;
        Secret = config[$"{configKey}:Secret"] ?? throw error;

        var scopes = new List<string>();
        config.GetSection($"{configKey}:Scopes").Bind(scopes);

        Scopes = scopes.ToArray();
    }

    [HttpGet("Logout")]
    public IActionResult Logout(bool web = true)
    {
        if (web)
        {
            HttpContext.Response.Cookies.Delete(AuthenticationHelper.AuthKey);
            return Redirect(Config.WebUrl);
        }

        return StatusCode(HttpStatusCode.NotImplemented);
    }
}