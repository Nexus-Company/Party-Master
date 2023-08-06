using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Nexus.Party.Master.Domain.Services;
public class InteractService : BackgroundService
{
    private readonly ILogger<InteractService> _logger;

    public InteractService(ILogger<InteractService> logger)
    {
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
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