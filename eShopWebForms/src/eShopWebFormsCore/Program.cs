using Autofac.Extensions.DependencyInjection;
using Autofac;
using eShopWebForms.Modules;
using eShopWebFormsCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAppConfig("eShop.config", isOptional: false);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterModule(new ApplicationModule(useMockData: false, useAzureStorage: false, useManagedIdentity: false));
});

builder.Services.AddConfigurationManager();

var app = builder.Build();

app.MapGet("/", () => "Hello World!")
    .AddHandlerPropertyInjection();

app.Run();
