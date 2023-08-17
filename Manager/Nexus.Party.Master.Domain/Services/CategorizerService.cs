using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Nexus.Party.Master.Domain.Services;
public class CategorizerService : BackgroundService
{
    private readonly ILogger<SyncService> _logger;
    private readonly ISyncService syncService;
    public CategorizerService(ILogger<SyncService> logger, ISyncService syncService)
    {
        _logger = logger;
        this.syncService = syncService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (syncService.SpotifyClient == null)
                await Task.Delay(1000, stoppingToken);
        }
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Categorizer Service is starting asynchronously.");

        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Categorizer Service is stopping asynchronously.");

        await base.StopAsync(cancellationToken);
    }
}