using Nexus.Spotify.Client;
using System.Xml.Serialization;

namespace Nexus.Party.Master.Api.OAuth.Controllers;

[AllowAnonymous]
[ApiController, Route("Spotify")]
public class SpotifyOAuthController : OAuthController
{
    private const string ConfigKey = "Spotify";
    private const string RedirectUri = "https://localhost:7191/spotify/callback";

    public SpotifyOAuthController(IServiceProvider serviceProvider, IConfiguration config)
        : base(serviceProvider, config, ConfigKey)
    {

    }

    [HttpGet, Route("Callback")]
    public async Task<IActionResult> Connect(string code, string state)
    {
        if (string.IsNullOrEmpty(code) ||
            string.IsNullOrEmpty(state) ||
            state != State)
            return BadRequest();

        var credential = await OAuthCredential.GetCredentialAsync(ClientId, Secret, code, Scopes, RedirectUri);

        SyncService!.SetClient(new(credential));

        State = Guid.NewGuid().ToString();

        return Redirect("http://localhost:5070/");
    }

    [HttpGet, Route("Authorize")]
    public IActionResult GenerateUrl()
        => Redirect(OAuthCredential.GenerateOAuthLink(ClientId, State, Scopes, RedirectUri));
}