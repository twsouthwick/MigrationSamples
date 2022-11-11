using Autofac.Extensions.DependencyInjection;
using Autofac;
using eShopWebForms.Modules;
using eShopWebFormsCore;
using System.Web.Optimization;
using System.Web.UI.WebControls;
using System.Web.UI;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Configuration.AddAppConfig("eShop.config", isOptional: false);

builder.Services.AddSystemWebAdapters()
     .AddJsonSessionSerializer(options =>
     {
         options.RegisterKey<DateTime>("SessionStartTime");
         options.RegisterKey<string>("MachineName");
     })
    .AddHttpHandlers()
    .AddDynamicPages(options =>
    {
        options.Files = builder.Environment.ContentRootFileProvider;
        options.UseFrameworkParser = true;
        options.AddAssemblyFrom<log4net.ILog>();

        options.AddTypeNamespace<ListView>("asp");
        options.AddTypeNamespace<ScriptManager>("asp");
        options.AddTypeNamespace<BundleReference>("webopt");
    })
    .AddRemoteAppClient(options =>
    {
        builder.Configuration.GetSection("RemoteApp").Bind(options);
        options.RemoteAppUrl = new(builder.Configuration["ReverseProxy:Clusters:fallbackCluster:Destinations:fallbackApp:Address"]!);
    })
    .AddSessionClient(options =>
    {
    });

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterModule(new ApplicationModule(useMockData: false, useAzureStorage: false, useManagedIdentity: false));
});

builder.Services.AddConfigurationManager();

var app = builder.Build();

app.UseWebFormsScripts();

app.UseRouting();

app.UseSystemWebAdapters();

app.MapHttpHandlers()
    .AddHandlerPropertyInjection();

app.MapReverseProxy();

app.Run();
