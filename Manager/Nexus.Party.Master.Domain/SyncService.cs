using System.Net.Http.Headers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nexus.Party.Master.Domain.Models.Spotify;

namespace Nexus.Party.Master.Domain;

public class SyncService : BackgroundService
{
    private readonly ILogger<SyncService> _logger;
    private string? _lastMusicId = null;

    public delegate void NewMusic(object sender, EventArgs args);

    public event NewMusic? MusicChange;
    public OAuthCredential? Credential { get; set; }
    private HttpClient Client { get; } = new();
    public Track? LastTrack { get; set; }

    public SyncService(ILogger<SyncService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (Credential == null)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                continue;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(500), stoppingToken);

            var status = await GetStatus(stoppingToken);

            LastTrack ??= status.Item;

            if (LastTrack.Id == (status.Item?.Id ?? string.Empty))
                continue;

            MusicChange?.Invoke(status.Item, null!);

            LastTrack = status.Item;
        }
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Music Sync Service is starting asynchronously.");

        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Music Sync Service is stopping asynchronously.");

        await base.StopAsync(cancellationToken);
    }

    private async Task<State> GetStatus(CancellationToken stoppingToken)
    {
        var request = new HttpRequestMessage()
        {
            RequestUri = new Uri("https://api.spotify.com/v1/me/player")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue(Credential!.Type, Credential.Token);

        var response = await Client.SendAsync(request, stoppingToken);

        string state = await response.Content.ReadAsStringAsync(stoppingToken);

        return JsonConvert.DeserializeObject<State>(state)!;
    }
}