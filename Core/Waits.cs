using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Serilog;

namespace EhuTests.NUnit.Core;

public static class Waits
{
    public static IWebElement UntilVisible(IWebDriver driver, By locator, int timeoutSec = 10)
    {
        TestLog.Logger.Debug("Waiting up to {Timeout}s for visibility of {Locator}", timeoutSec, locator);
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSec));
        return wait.Until(d =>
        {
            var el = d.FindElement(locator);
            return el.Displayed ? el : null!;
        });
    }

    public static bool UntilPageContainsText(IWebDriver driver, string text, int timeoutSec = 10)
    {
        TestLog.Logger.Debug("Waiting up to {Timeout}s for page to contain text '{Text}'", timeoutSec, text);
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSec));
        var result = wait.Until(d => d.PageSource.Contains(text, StringComparison.CurrentCultureIgnoreCase));
        TestLog.Logger.Debug("Page contains text '{Text}': {Result}", text, result);
        return result;
    }
}
