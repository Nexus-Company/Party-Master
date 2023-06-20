using Nexus.Party.Master.Domain;

namespace Nexus.Party.Master.Api.Controllers.Base;

public class UseSyncController : BaseController
{
    private protected readonly SyncService SyncService;
    private protected UseSyncController(IServiceProvider serviceProvider)
    {
        SyncService = serviceProvider.GetService<SyncService>()!;
    }
}

public partial class OAuthController
{
    private protected readonly SyncService? SyncService;
    private protected OAuthController(IServiceProvider serviceProvider, IConfiguration config, string configKey)
        : this(config,configKey)
    {
        SyncService = serviceProvider.GetService<SyncService>()!;
    }
}