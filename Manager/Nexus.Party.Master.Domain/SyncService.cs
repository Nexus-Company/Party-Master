using System.Net.Http.Headers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nexus.Party.Master.Domain.Models.Spotify;

namespace Nexus.Party.Master.Domain;

public partial class SyncService : BackgroundService
{
    private readonly ILogger<SyncService> _logger;

    public delegate void NewMusic(object sender, EventArgs args);

    public event NewMusic? MusicChange;
    public OAuthCredential? Credential { get; set; }
    private HttpClient Client { get; } = new();
    public Track? Track { get; set; }

    public SyncService(ILogger<SyncService> logger)
    {
        _logger = logger;
        Queue = new List<Track>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        bool first = false;
        while (!stoppingToken.IsCancellationRequested)
        {
            if (Credential == null)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                first = true;
                continue;
            }

            if (first)
            {
                await DefineQueueAsync(stoppingToken);
                first = false;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(500), stoppingToken);

            var status = await GetStatus(stoppingToken);

            Track ??= status.Item;

            if (Track.Id == (status.Item?.Id ?? string.Empty))
                continue;

            await DefineQueueAsync(stoppingToken);

            MusicChange?.Invoke(status.Item, null!);

            Track = status.Item;
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
}