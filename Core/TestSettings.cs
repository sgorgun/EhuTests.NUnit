namespace EhuTests.NUnit.Core;

public sealed class TestSettings
{
    public UrlsSection Urls { get; init; } = null!;
    public string Browser { get; init; } = null!;
    public bool Headless { get; init; }
    public string[]? BrowserArgs { get; init; }
    public int ImplicitWaitSec { get; init; }
    public WaitsSection Waits { get; init; } = null!;
    public RetrySection Retry { get; init; } = null!;
    public bool Parallel { get; init; }
    public string[]? LocalizationHosts { get; init; }
    public DriverSection Driver { get; init; } = null!;
    public LoggingSection Logging { get; init; } = null!;
    public ClickSection Click { get; init; } = null!;
}

public sealed class UrlsSection
{
    public string BaseEn { get; init; } = null!;
    public string BaseLt { get; init; } = null!;
    public string SearchTemplate { get; init; } = null!;
}

public sealed class WaitsSection
{
    public int SearchResultsTextTimeoutSec { get; init; }
    public int DefaultExplicitWaitSec { get; init; }
}

public sealed class RetrySection
{
    public int Navigation { get; init; }
    public int Search { get; init; }
    public int Localization { get; init; }
}

public sealed class DriverSection
{
    public bool ReusePerTest { get; init; }
}

public sealed class LoggingSection
{
    public bool Verbose { get; init; }
}

public sealed class ClickSection
{
    public bool UseJavaScript { get; init; }
}
