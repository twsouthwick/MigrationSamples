using System.Web.UI;
using System.Web.UI.WebControls;
using eShopWebForms;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using eShopWebForms.Modules;
using System.Web;
using System.Web.Optimization;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAppConfig("eShop.config", isOptional: false);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterModule(new ApplicationModule(useMockData: false, useAzureStorage: false, useManagedIdentity: false));
});

builder.Services.AddConfigurationManager();
builder.Services.AddAuthenticationCore();
builder.Services.AddSystemWebAdapters()
    .AddJsonSessionSerializer(options =>
    {
        options.RegisterKey<DateTime>("SessionStartTime");
        options.RegisterKey<string>("MachineName");
    })
    .AddDynamicPages(options =>
    {
        options.UseFrameworkParser = false;
        options.AddAssemblyFrom<log4net.ILog>();

        options.AddTypeNamespace<ListView>("asp");
        options.AddTypeNamespace<ScriptManager>("asp");
        options.AddTypeNamespace<BundleReference>("webopt");
    })
    .AddOptimization(options =>
    {
        BundleConfig.RegisterBundles(options.Bundles);
    })
    .AddRemoteAppClient(options =>
    {
        builder.Configuration.GetSection("RemoteApp").Bind(options);
        options.RemoteAppUrl = new(builder.Configuration["ReverseProxy:Clusters:fallbackCluster:Destinations:fallbackApp:Address"]);
    })
    //.AddAuthenticationClient(isDefaultScheme: true, options =>
    //{
    //})
    .AddSessionClient(options =>
    {
    });

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseWebFormsScripts();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSystemWebAdapters();

app.MapDynamicAspxPages(app.Environment.ContentRootFileProvider);
//app.MapReverseProxy();

app.Run();
