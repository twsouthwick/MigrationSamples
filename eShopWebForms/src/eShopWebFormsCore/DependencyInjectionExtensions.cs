using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SystemWebAdapters;

namespace eShopWebFormsCore;

internal static class DependencyInjectionExtensions
{
    public static T AddHandlerPropertyInjection<T>(this T builder)
        where T : IEndpointConventionBuilder
    {
        return builder.AddEndpointFilter(new AutofacPropertyHandlerInjectionFilter());
    }

    private class AutofacPropertyHandlerInjectionFilter : IEndpointFilter
    {
        public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            if (context.HttpContext.Features.Get<IHttpHandlerFeature>() is { Current: { } handler })
            {
                context.HttpContext.RequestServices.GetAutofacRoot().InjectUnsetProperties(handler);
            }

            return next(context);
        }
    }
}
