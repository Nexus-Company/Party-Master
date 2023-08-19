using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nexus.Party.Master.Dal;
using Nexus.Spotify.Client;
using Nexus.Spotify.Client.Models;

namespace Nexus.Party.Master.Domain.Services;

public partial class SyncService : BackgroundService, ISyncService
{
    private readonly ILogger<SyncService> _logger;
    private DateTime started;
    SpotifyClient? ISyncService.SpotifyClient { get => SpotClient; }
    private SpotifyClient? SpotClient;

    public event ISyncService.NewMusic? MusicChange;
    public SpotifyClient? SpotifyClient { get; set; }
    public PlayerState? Player { get; set; }
    public Track? Track { get; private set; }
    public DateTime? Started { get => started; }

    public IEnumerable<Track> Queue { get; private set; } = Array.Empty<Track>();
    public bool Online { get; private set; }

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
            if (((ISyncService)this).SpotifyClient == null)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                first = true;
                continue;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(500), stoppingToken);
            try
            {
                Player = await SpotClient.GetStatusAsync(stoppingToken);

                if (Player == null)
                {
                    Online = false;
                    continue;
                }

                if (first && Player.IsPlaying)
                {
                    Queue = await SpotClient.GetQueueAsync(stoppingToken);
                    first = false;
                }

                Online = true;
                Track ??= Player.Item;

                if (Track.Id == (Player.Item?.Id ?? string.Empty))
                    continue;

                Queue = await SpotClient.GetQueueAsync(stoppingToken);
                Track = Player.Item;
                started = DateTime.UtcNow;

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

    public async Task AddTrackInQueueAsync(Track track)
    {
        await SpotClient!.AddToQueueAsync(track.Id, Player!.Device.Id);
    }

    public void SetClient(SpotifyClient client)
        => SpotClient = client;
}

public interface ISyncService : IHostedService
{
    public delegate void NewMusic(object sender, EventArgs args);

    public event NewMusic? MusicChange;

    internal SpotifyClient? SpotifyClient { get; }

    public IEnumerable<Track> Queue { get; }

    public PlayerState Player { get; set; }

    public bool Online { get; }

    public Track? Track { get; }

    public DateTime? Started { get; }
    public void SetClient(SpotifyClient client);
    public Task AddTrackInQueueAsync(Track track);
}