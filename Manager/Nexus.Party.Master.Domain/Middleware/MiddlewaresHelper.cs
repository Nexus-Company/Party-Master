using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Nexus.Party.Master.Dal;
using System.Reflection;
using Nexus.Party.Master.Domain.Models;

namespace Nexus.Party.Master.Domain.Middleware;

public static class MiddlewaresHelper
{
    public static IApplicationBuilder UseInteract(this IApplicationBuilder app, AuthenticationContext authCtx, Config config)
    {
        return app.UseMiddleware<InteractMiddleware>(new object[] { authCtx, config });
    }

    internal static TAttribute? TryGetAttribute<TAttribute>(HttpContext ctx, bool controller, bool inherit)
    {
        var controllerActionDescriptor = (ctx.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>()) ??
            throw new Exception("Cannot get attribute of type: " + typeof(TAttribute)!.Name);

        if (controllerActionDescriptor == null)
            return (TAttribute?)(object?)null;

        object? customAttribute = controllerActionDescriptor.MethodInfo.GetCustomAttribute(typeof(TAttribute), inherit);
        if (customAttribute == null && controller)
        {
            customAttribute = controllerActionDescriptor.ControllerTypeInfo.GetCustomAttribute(typeof(TAttribute), inherit);
        }

        return (TAttribute?)customAttribute;
    }
}