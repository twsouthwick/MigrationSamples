var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSystemWebAdapters()
    .AddJsonSessionSerializer(options =>
    {
        options.RegisterKey<DateTime>("SessionStartTime");
        options.RegisterKey<string>("MachineName");
    })
    .AddDynamicPages(options =>
    {
        options.UseFrameworkParser = true;
    })
    .AddRemoteAppClient(options =>
    {
        builder.Configuration.GetSection("RemoteApp").Bind(options);
        options.RemoteAppUrl = new(builder.Configuration["ReverseProxy:Clusters:fallbackCluster:Destinations:fallbackApp:Address"]);
    })
    .AddAuthenticationClient(isDefaultScheme: true, options =>
    {
    })
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
app.MapReverseProxy();

app.Run();
