using Microsoft.AspNetCore.Mvc;
using Nexus.Party.Master.Domain.Models.Spotify;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Nexus.Party.Master.Api.Controllers.Base;
public partial class OAuthController : BaseController
{
    private protected static string? State { get; set; }
    private protected string ClientId { get; set; }
    private protected string Secret { get; set; }
    private protected string[] Scopes { get; set; }

    private protected OAuthController(IConfiguration config, string configKey)
        : base()
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

}
