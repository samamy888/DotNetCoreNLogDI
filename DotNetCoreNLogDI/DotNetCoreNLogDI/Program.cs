using DotNetCoreNLogDI;
using NLog.Extensions.Logging;

static void ConfigureServices(IServiceCollection services)
{
    // build config
    IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

    // configure logging
    services.AddLogging(builder =>
    {
        builder.ClearProviders();
        builder.AddConsole();
        builder.AddDebug();
        builder.AddNLog(config.GetSection("NLog"));
    });

    // add app
    services.AddTransient<App>();
}

var services = new ServiceCollection();
ConfigureServices(services);

using var serviceProvider = services.BuildServiceProvider();

// entry to run app
await serviceProvider.GetService<App>()!.Run();