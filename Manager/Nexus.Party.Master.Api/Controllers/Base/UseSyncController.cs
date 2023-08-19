using Nexus.Party.Master.Domain.Services;

namespace Nexus.Party.Master.Api.Controllers.Base;

public class UseSyncController : BaseController
{
    private protected readonly ISyncService SyncService;
    private protected readonly CategorizerService CategorizerService;
    private protected UseSyncController(IConfiguration config, IServiceProvider serviceProvider)
        : base(config)
    {
        SyncService = serviceProvider.GetService<ISyncService>()!;
        CategorizerService = serviceProvider.GetService<CategorizerService>()!;
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