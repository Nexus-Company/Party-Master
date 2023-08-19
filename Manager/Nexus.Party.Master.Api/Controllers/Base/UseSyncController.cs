using Nexus.Party.Master.Domain.Services;

namespace Nexus.Party.Master.Api.Controllers.Base;

public class UseSyncController : BaseController
{
    private protected readonly CategorizerService categorizerService;
    private protected readonly ISyncService syncService;
    public int Connecteds
        => interContext.Connecteds.Count();
    private protected UseSyncController(IConfiguration config, IServiceProvider serviceProvider)
        : base(config)
    {
        syncService = serviceProvider.GetService<ISyncService>()!;
        categorizerService = serviceProvider.GetService<CategorizerService>()!;
    }
}

public partial class OAuthController
{
    private protected readonly ISyncService? SyncService;
    private protected OAuthController(IServiceProvider serviceProvider, IConfiguration config, string configKey)
        : this(config, configKey)
    {
        SyncService = serviceProvider.GetService<ISyncService>();
    }
}