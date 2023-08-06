using Nexus.Spotify.Client;

namespace Nexus.Party.Master.Api.OAuth.Controllers;

[AllowAnonymous]
[ApiController, Route("Spotify")]
public class SpotifyOAuthController : OAuthController
{
    private const string ConfigKey = "Spotify";
    private const string RedirectUri = "https://localhost:44383/spotify/callback";

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

        using HttpClient client = new();

        var request = new HttpRequestMessage()
        {
            RequestUri = new Uri(OAuthCredential.TokenEndpoint),
            Method = HttpMethod.Post,
            Content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "grant_type", "authorization_code" },
                { "client_id", ClientId },
                { "client_secret", Secret },
                { "code", code },
                { "scope", string.Join(" ", Scopes) },
                { "redirect_uri", RedirectUri }
            })
        };

        var response = await client.SendAsync(request);

        var text = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return BadRequest(text);

        var credential = JsonConvert.DeserializeObject<OAuthCredential>(text) ??
                                   throw new ArgumentException("Authentication OAuth 2.0 failed.");


        SyncService!.SpotifyClient = new(credential);
        State = Guid.NewGuid().ToString();
        credential
            .ConfigureRefresh(ClientId, Secret, Scopes);

        return Redirect("https://localhost:44370/");
    }

    [HttpGet, Route("Authorize")]
    public IActionResult GenerateUrl()
        => Redirect($"https://accounts.spotify.com/pt-BR/authorize?" +
                    $"client_id={ClientId}&" +
                    $"redirect_uri=https://localhost:44383/spotify/callback&" +
                    $"response_type=code&" +
                    $"state={State}&" +
                    $"scope={string.Join(" ", Scopes)}");
}