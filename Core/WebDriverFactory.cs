using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Serilog;

namespace EhuTests.NUnit.Core;

public static class WebDriverFactory
{
    public static IWebDriver Create(Browser browser, OptionsBuilder builder)
    {
        TestLog.Logger.Debug("Creating WebDriver for {Browser}", browser);
        return browser switch
        {
            Browser.Chrome => CreateChrome(builder),
            _ => throw new NotSupportedException($"Browser {browser} not supported.")
        };
    }

    private static ChromeDriver CreateChrome(OptionsBuilder builder)
    {
        var options = builder.BuildChromeOptions();
        var driver = new ChromeDriver(options);
        TestLog.Logger.Information("ChromeDriver created.");

        var implicitWait = builder.ImplicitWaitSeconds();
        if (implicitWait.HasValue)
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(implicitWait.Value);
            TestLog.Logger.Debug("Applied implicit wait of {Seconds}s", implicitWait.Value);
        }

        return driver;
    }
}
