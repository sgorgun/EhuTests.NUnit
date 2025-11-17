using Serilog;
using Serilog.Events;

namespace EhuTests.NUnit.Core;

public static class TestLog
{
    private static bool _initialized;

    public static ILogger Logger => Serilog.Log.Logger;

    public static void Initialize(TestSettings settings)
    {
        if (_initialized) return;

        var config = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId()
            .Enrich.WithEnvironmentUserName()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .WriteTo.File(
                path: Path.Combine(AppContext.BaseDirectory, "logs", "tests.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                restrictedToMinimumLevel: LogEventLevel.Debug,
                outputTemplate: "{Timestamp:O} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");

        if (settings.Logging.Verbose)
        {
            config.MinimumLevel.Verbose();
        }

        Serilog.Log.Logger = config.CreateLogger();
        _initialized = true;

        Logger.Information("Logger initialized. Browser={Browser} Headless={Headless}", settings.Browser, settings.Headless);
    }

    public static void Shutdown()
    {
        if (!_initialized) return;
        Logger.Information("Logger shutting down.");
        Serilog.Log.CloseAndFlush();
        _initialized = false;
    }
}
