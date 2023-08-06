global using Newtonsoft.Json;
using Nexus.Spotify.Client.Models;
namespace Nexus.Spotify.Client;

public class SpotifyClient
{
    const string SpotifyEndPoint = "https://api.spotify.com/v1";
    internal const string PlayerUrl = $"{SpotifyEndPoint}/me/player";
    internal HttpClient HttpClient { get; } = new();
    internal OAuthCredential Credential { get => _credential; }
    private readonly OAuthCredential _credential;

    public SpotifyClient(OAuthCredential credential)
    {
        _credential = credential;
    }
    public async Task<PlayerState> GetStatusAsync(CancellationToken? stoppingToken)
    {
        stoppingToken ??= CancellationToken.None;
        var request = new HttpRequestMessage()
        {
            RequestUri = new Uri(PlayerUrl)
        };
        request.Headers.Authorization = Credential!.GetHeader();

        var response = await HttpClient.SendAsync(request, stoppingToken.Value);

        string contentStr = await response.Content.ReadAsStringAsync(stoppingToken.Value);
        var state = JsonConvert.DeserializeObject<PlayerState>(contentStr)!;

        state.client = this;

        return state;
    }

    public async Task<IEnumerable<Track>> GetQueueAsync(CancellationToken? stoppingToken)
    {
        stoppingToken ??= CancellationToken.None;
        var request = CreateRequest($"{PlayerUrl}/queue");

        var response = await HttpClient.SendAsync(request, stoppingToken.Value);

        string state = await response.Content.ReadAsStringAsync(stoppingToken.Value);

        var rst = JsonConvert.DeserializeObject<QueueResult>(state)!;

        return rst.Queue;
    }

    internal HttpRequestMessage CreateRequest(string uri)
    {
        var request = new HttpRequestMessage()
        {
            RequestUri = new Uri(uri),
        };
        request.Headers.Authorization = Credential!.GetHeader();
        return request;
    }

    private class QueueResult
    {
        public IEnumerable<Track> Queue { get; set; }
    }
}