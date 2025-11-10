using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace EhuTests.NUnit.Core;

public static class Waits
{
    public static IWebElement UntilVisible(IWebDriver driver, By locator, int timeoutSec = 10)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSec));
        return wait.Until(d =>
        {
            var el = d.FindElement(locator);
            return el.Displayed ? el : null!;
        });
    }

    public static bool UntilPageContainsText(IWebDriver driver, string text, int timeoutSec = 10)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSec));
        return wait.Until(d => d.PageSource.Contains(text, StringComparison.CurrentCultureIgnoreCase));
    }
}
