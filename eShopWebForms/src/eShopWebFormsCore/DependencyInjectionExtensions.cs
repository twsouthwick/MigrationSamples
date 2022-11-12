using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SystemWebAdapters;

namespace eShopWebFormsCore;

internal static class DependencyInjectionExtensions
{
    public static T AddHandlerPropertyInjection<T>(this T builder)
        where T : IEndpointConventionBuilder
    {
        builder.Add(builder =>
        {
            var next = builder.RequestDelegate;

            if (next is not null)
            {
                builder.RequestDelegate = async (HttpContext context) =>
                {
                    var log = context.RequestServices.GetRequiredService<ILogger<System.Web.UI.Page>>();

                    if (context.Features.Get<IHttpHandlerFeature>() is { Current: { } handler })
                    {
                        context.RequestServices.GetAutofacRoot().InjectUnsetProperties(handler);
                    }

                    await next(context);
                };
            }
        });
        return builder;
    }
}
