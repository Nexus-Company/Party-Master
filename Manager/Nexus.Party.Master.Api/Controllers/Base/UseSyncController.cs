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