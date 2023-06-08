using Nexus.Party.Master.Domain.Models.Spotify;

namespace Nexus.Party.Master.Api.OAuth.Controllers;

[ApiController, Route("Spotify")]
public class SpotifyOAuthController : UseSyncController
{
    private const string ConfigKey = "Spotify";
    private const string RedirectUri = "https://localhost:44383/spotify/callback";
    private string ClientId { get; set; }
    private string Secret { get; set; }
    private static string? State { get; set; }

    private string[] Scopes { get; set; }

    public SpotifyOAuthController(IServiceProvider serviceProvider, IConfiguration config)
        : base(serviceProvider)
    {
        Exception error =
            new ArgumentException(
                "It's not possible start this application without Spotify ClientId, ClientSecret and Scopes.");

        ClientId = config[$"{ConfigKey}:ClientId"] ?? throw error;
        Secret = config[$"{ConfigKey}:Secret"] ?? throw error;

        var scopes = new List<string>();
        config.GetSection($"{ConfigKey}:Scopes").Bind(scopes);

        Scopes = scopes.ToArray();
        State ??= Guid.NewGuid().ToString();
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

        SyncService.Credential = JsonConvert.DeserializeObject<OAuthCredential>(text) ??
                                  throw new ArgumentException("Authentication OAuth 2.0 failed.");

        State = Guid.NewGuid().ToString();
        SyncService
            .Credential
            .ConfigureRefresh(ClientId, Secret, Scopes);

        return Ok();
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