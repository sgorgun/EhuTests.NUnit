using NUnit.Framework;
using EhuTests.NUnit.Core;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;

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
    }

    [SetUp]
    public void SetUp()
    {
        var builder = OptionsBuilder.ForChrome()
            .Headless(Settings.Headless)
            .WithImplicitWait(Settings.ImplicitWaitSec);

        if (Settings.BrowserArgs != null)
            foreach (var arg in Settings.BrowserArgs) builder.WithArg(arg);

        var browser = Enum.TryParse<Browser>(Settings.Browser, true, out var b) ? b : Browser.Chrome;
        DriverManager.Instance.Initialize(browser, builder);

        try { DriverManager.Instance.Driver.Manage().Cookies.DeleteAllCookies(); } catch { }
    }

    [TearDown]
    public void TearDown()
    {
        DriverManager.Instance.Quit();
    }
}
