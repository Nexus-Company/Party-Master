using System.Net.Http.Headers;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Nexus.Spotify.Client;
public class OAuthCredential
{
    public const string TokenEndpoint = "https://accounts.spotify.com/api/token";

    private string
        _clientId = null!,
        _secret = null!;

    private string[] _scopes = null!;
    [JsonProperty("access_token")] public string Token { get; set; } = null!;
    [JsonProperty("token_type")] public string Type { get; set; } = null!;
    [JsonProperty("refresh_token")] private string Refresh { get; set; } = null!;
    [JsonProperty("expires_in")] public double ExpiresIn { get; set; }


    public void ConfigureRefresh(string clientId, string secret, string[] scopes)
    {
        _clientId = clientId;
        _secret = secret;
        _scopes = scopes;

        var timer = new Timer((ExpiresIn - 10) * 1000);
        timer.Elapsed += RefreshToken;
        timer.Start();
    }

    private async void RefreshToken(object? sender, ElapsedEventArgs elapsedEventArgs)
    {
        using HttpClient client = new();

        var request = new HttpRequestMessage()
        {
            RequestUri = new Uri(TokenEndpoint),
            Method = HttpMethod.Post,
            Content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "grant_type", "refresh_token" },
                { "client_id", _clientId },
                { "client_secret", _secret },
                { "refresh_token", Refresh }
            })
        };

        var response = await client.SendAsync(request);

        var obj = JsonConvert.DeserializeObject<OAuthCredential>(await response.Content.ReadAsStringAsync());

        Token = obj!.Token;
        Refresh = obj!.Refresh;
        ExpiresIn = obj!.ExpiresIn;
    }

    internal AuthenticationHeaderValue GetHeader()
        => new(Type, Token);

    public override string ToString()
        => $"{Type}  {Token}";
}