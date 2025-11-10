using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace EhuTests.NUnit.Core;

public static class WebDriverFactory
{
    public static IWebDriver Create(Browser browser, OptionsBuilder builder)
    {
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

        var implicitWait = builder.ImplicitWaitSeconds();
        if (implicitWait.HasValue)
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(implicitWait.Value);

        return driver;
    }
}
