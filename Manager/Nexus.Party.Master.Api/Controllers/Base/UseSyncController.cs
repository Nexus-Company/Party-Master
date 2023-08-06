﻿using Nexus.Party.Master.Domain.Services;

namespace Nexus.Party.Master.Api.Controllers.Base;

public class UseSyncController : BaseController
{
    private protected readonly SyncService syncService;
    private protected readonly InteractService interactService;
    public int Connecteds
        => interContext.Connecteds.Count();
    private protected UseSyncController(IConfiguration config, IServiceProvider serviceProvider)
        : base(config)
    {
        syncService = serviceProvider.GetService<SyncService>()!;
        interactService = serviceProvider.GetService<InteractService>()!;
    }
}

public partial class OAuthController
{
    private protected readonly SyncService? SyncService;
    private protected OAuthController(IServiceProvider serviceProvider, IConfiguration config, string configKey)
        : this(config, configKey)
    {
        SyncService = serviceProvider.GetService<SyncService>()!;
    }
}