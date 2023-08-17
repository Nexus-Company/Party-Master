using Microsoft.Extensions.Hosting;

namespace Nexus.Party.Master.Domain.Services;
public class CategorizerService : BackgroundService
{
    private ISyncService syncService;
    public CategorizerService(ISyncService syncService)
    {
        this.syncService = syncService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {

        }
    }
}