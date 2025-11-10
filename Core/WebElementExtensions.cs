using OpenQA.Selenium;

namespace EhuTests.NUnit.Core;

public static class WebElementExtensions
{
    public static void SafeClick(this IWebElement element, IWebDriver driver)
    {
        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
    }
}
