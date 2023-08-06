﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nexus.Spotify.Client;
using Nexus.Spotify.Client.Models;

namespace Nexus.Party.Master.Domain.Spotify;

public partial class SyncService : BackgroundService
{
    private readonly ILogger<SyncService> _logger;

    public delegate void NewMusic(object sender, EventArgs args);

    public event NewMusic? MusicChange;
    public SpotifyClient? SpotifyClient { get; set; }
    public Track? Track { get; private set; }
    public IEnumerable<Track> Queue { get; private set; }
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
            if (SpotifyClient == null)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                first = true;
                continue;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(500), stoppingToken);

            try
            {
                var status = await SpotifyClient.GetStatusAsync(stoppingToken);

                if (status == null)
                {
                    Online = false;
                    continue;
                }

                if (first && status.IsPlaying)
                {
                    Queue = await SpotifyClient.GetQueueAsync(stoppingToken);
                    first = false;
                }

                Online = true;
                Track ??= status.Item;

                if (Track.Id == (status.Item?.Id ?? string.Empty))
                    continue;

                Queue = await SpotifyClient.GetQueueAsync(stoppingToken);

                MusicChange?.Invoke(status!.Item, null!);

                Track = status.Item;
            }
            catch (Exception ex)
            {
                Online = false;
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