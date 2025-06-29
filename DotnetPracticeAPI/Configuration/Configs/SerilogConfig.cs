using Serilog;

namespace Configuration.Configs
{
    public static class SerilogConfig
    {
        public static Serilog.ILogger CreateLogger()
        {
            return new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}
