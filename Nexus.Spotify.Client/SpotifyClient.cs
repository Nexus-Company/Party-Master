global using Newtonsoft.Json;
using Nexus.Spotify.Client.Models;
namespace Nexus.Spotify.Client;

public class SpotifyClient : IDisposable
{
    const string SpotifyEndPoint = "https://api.spotify.com/v1";
    const string PlayerUrl = $"{SpotifyEndPoint}/me/player";
    internal HttpClient HttpClient { get; } = new();
    internal OAuthCredential Credential { get => _credential; }
    private readonly OAuthCredential _credential;

    public SpotifyClient(OAuthCredential credential)
    {
        _credential = credential;
    }
    public async Task<State> GetStatusAsync(CancellationToken? stoppingToken = null)
    {
        stoppingToken ??= CancellationToken.None;
        var request = new HttpRequestMessage()
        {
            RequestUri = new Uri(PlayerUrl)
        };
        request.Headers.Authorization = Credential!.GetHeader();

        var response = await HttpClient.SendAsync(request, stoppingToken.Value);

        string state = await response.Content.ReadAsStringAsync(stoppingToken.Value);

        return LoadModel(JsonConvert.DeserializeObject<State>(state)!);
    }

    public async Task<IEnumerable<Track>> GetQueueAsync(CancellationToken? stoppingToken = null)
    {
        stoppingToken ??= CancellationToken.None;
        var request = CreateRequest($"{PlayerUrl}/queue");

        var response = await HttpClient.SendAsync(request, stoppingToken.Value);

        string state = await response.Content.ReadAsStringAsync(stoppingToken.Value);

        var rst = JsonConvert.DeserializeObject<QueueResult>(state)!;

        return rst.Queue;
    }

    public async Task<Track> GetTrackAsync(string id, CancellationToken? stoppingToken = null)
    {
        stoppingToken ??= CancellationToken.None;
        var request = CreateRequest($"{SpotifyEndPoint}/tracks/{id}");

        var response = await HttpClient.SendAsync(request, stoppingToken.Value);

        string state = await response.Content.ReadAsStringAsync(stoppingToken.Value);

        return LoadModel(JsonConvert.DeserializeObject<Track>(state)!);
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

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    private T LoadModel<T>(T md) where T : Model
    {
        md.SpotifyClient = this;
        return md;
    }
    private class QueueResult
    {
        public IEnumerable<Track> Queue { get; set; }
    }
}