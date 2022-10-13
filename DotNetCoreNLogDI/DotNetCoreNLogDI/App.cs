namespace DotNetCoreNLogDI
{
    public class App
    {
        private readonly ILogger<App> _logger;
        public App(ILogger<App> logger)
        {
            _logger = logger;
        }
        public async Task Run()
        {
            _logger.LogInformation("LogInformation");
            _logger.LogWarning("LogWarning");
        }
    }
}
