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

    protected static TestSettings TestSettings => Settings;

    [OneTimeSetUp]
    public void LoadConfig()
    {
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
        var result = TestContext.CurrentContext.Result;
        var testName = TestContext.CurrentContext.Test.Name;
        if (result.FailCount > 0)
        {
            TestLog.Logger.Error("Test {TestName} failed. Status={Status} Message={Message}",
                testName, result.Outcome.Status, result.Message);
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
        TestLog.Logger.Information("All tests finished.");
        TestLog.Shutdown();
    }
}
