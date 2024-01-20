using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Nexus.Party.Master.Dal;
using Nexus.Party.Master.Dal.Models.Accounts;
using Nexus.Party.Master.Domain.Models;
using Nexus.Party.Master.Domain.Services;
using Nexus.Stock.Domain.Helpers;

namespace Nexus.Party.Master.Domain.Middleware;
internal class InteractMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AuthenticationContext authCtx;
    private readonly Config _config;
    private readonly SyncService _syncService;
    private InteractContext interContext;

    public InteractMiddleware(RequestDelegate next, AuthenticationContext authCtx, Config config, SyncService syncService)
    {
        _next = next;
        interContext = new();
        this.authCtx = authCtx;
        _config = config;
        _syncService = syncService;
    }

    public async Task InvokeAsync(HttpContext ctx, IAuthenticationContextFactory auth)
    {
        try
        {
            var attr = MiddlewaresHelper.TryGetAttribute<InteractAttribute>(ctx, true, true);

            if (attr == null)
            {
                await _next(ctx);
                return;
            }

            if (!_syncService.Online)
            {
                // O Serviço está offline 
            }

            Account user = auth.Account!;

            int count = await interContext.Connecteds.CountAsync();

            if (count > _config.MinInteract)
            {
                // Numero minimo de participantes para interação não foi atigindo 
            }

            var connected = await (from con in interContext.Connecteds
                                   where con.AccountId == user.Id
                                   select con).FirstOrDefaultAsync();

            if (connected == null)
            {
                // O usuário deve estar acompanhando as musicas para participar 
            }
        }
        catch (Exception)
        {

        }

        await _next(ctx);
    }
}

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public sealed class InteractAttribute : Attribute
{
    public InteractAttribute()
    {
    }
}