using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nexus.Party.Master.Dal;
using Nexus.Spotify.Client;
using Nexus.Spotify.Client.Models;

namespace Nexus.Party.Master.Domain.Services;

public partial class SyncService : BackgroundService
{
    private readonly ILogger<SyncService> _logger;
    private readonly InteractContext interContext;

    public delegate void NewMusic(object sender, EventArgs args);

    public event NewMusic? MusicChange;
    public SpotifyClient? SpotifyClient { get; set; }
    public PlayerState? Player { get; set; }
    public Track? Track { get; private set; }
    public DateTime? Started { get; set; }
    public IEnumerable<Track> Queue { get; private set; } = Array.Empty<Track>();
    public bool Online { get; private set; }

    public SyncService(ILogger<SyncService> logger)
    {
        _logger = logger;
        Queue = new List<Track>();
        interContext = new InteractContext();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        bool first = false;
        while (!stoppingToken.IsCancellationRequested)
        {
            if (SpotifyClient == null)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                first = true;
                continue;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(500), stoppingToken);
            try
            {
                Player = await SpotifyClient.GetStatusAsync(stoppingToken);

                if (Player == null)
                {
                    Online = false;
                    continue;
                }

                if (first && Player.IsPlaying)
                {
                    Queue = await SpotifyClient.GetQueueAsync(stoppingToken);
                    first = false;
                }

                Online = true;
                Track ??= Player.Item;

                if (Track.Id == (Player.Item?.Id ?? string.Empty))
                    continue;

                Queue = await SpotifyClient.GetQueueAsync(stoppingToken);
                Track = Player.Item;
                Started = DateTime.UtcNow;

                MusicChange?.Invoke(Player!.Item!, null!);
            }
            catch (Exception)
            {
                Online = false;
                MusicChange = null;
            }
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