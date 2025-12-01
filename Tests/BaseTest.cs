using NUnit.Framework;
using EhuTests.NUnit.Core;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using Serilog;

namespace EhuTests.NUnit.Tests;

[Parallelizable(ParallelScope.All)]
public abstract class BaseTest
{
    private static TestSettings Settings = default!;
    private DateTime _testStartUtc;

    protected static TestSettings TestSettings => Settings;

    [OneTimeSetUp]
    public void LoadConfig()
    {
        ReportAggregator.SuiteStarted();

        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.test.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        Settings = config.GetSection("TestSettings").Get<TestSettings>()
            ?? throw new InvalidOperationException("Missing TestSettings configuration.");
        RuntimeConfig.Initialize(Settings);
        TestLog.Initialize(Settings);
        TestLog.Logger.Information("Configuration loaded.");
    }

    [SetUp]
    public void SetUp()
    {
        _testStartUtc = DateTime.UtcNow;
        TestLog.Logger.Debug("Test SetUp starting for {TestName}", TestContext.CurrentContext.Test.Name);

        var builder = OptionsBuilder.ForChrome()
            .Headless(Settings.Headless)
            .WithImplicitWait(Settings.ImplicitWaitSec);

        if (Settings.BrowserArgs != null)
            foreach (var arg in Settings.BrowserArgs)
            {
                builder.WithArg(arg);
                TestLog.Logger.Debug("Added browser arg {Arg}", arg);
            }

        var browser = Enum.TryParse<Browser>(Settings.Browser, true, out var b) ? b : Browser.Chrome;
        DriverManager.Instance.Initialize(browser, builder);
        TestLog.Logger.Information("Driver initialized for {Browser}", browser);

        try
        {
            DriverManager.Instance.Driver.Manage().Cookies.DeleteAllCookies();
            TestLog.Logger.Debug("Cookies cleared.");
        }
        catch (Exception ex)
        {
            TestLog.Logger.Warning(ex, "Failed to clear cookies.");
        }
    }

    [TearDown]
    public void TearDown()
    {
        var ctx = TestContext.CurrentContext;
        var status = ctx.Result.Outcome.Status.ToString();
        var testName = ctx.Test.Name;
        var durationMs = (DateTime.UtcNow - _testStartUtc).TotalMilliseconds;
        ReportAggregator.Add(testName, status, durationMs);

        if (ctx.Result.FailCount > 0)
        {
            TestLog.Logger.Error("Test {TestName} failed. Status={Status} Message={Message}",
                testName, ctx.Result.Outcome.Status, ctx.Result.Message);
        }
        else
        {
            TestLog.Logger.Information("Test {TestName} finished successfully.", testName);
        }

        DriverManager.Instance.Quit();
        TestLog.Logger.Debug("Driver quit.");
    }

    [OneTimeTearDown]
    public void GlobalTearDown()
    {
        var outDir = Path.Combine(AppContext.BaseDirectory, "reports");
        ReportAggregator.WriteSummary(outDir);
        TestLog.Logger.Information("Summary report written to {Dir}", outDir);

        TestLog.Logger.Information("All tests finished.");
        TestLog.Shutdown();
    }
}
