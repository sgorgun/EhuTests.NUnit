using OpenQA.Selenium;
using Serilog;

namespace EhuTests.NUnit.Core;

public static class WebElementExtensions
{
    public static void SafeClick(this IWebElement element, IWebDriver driver)
    {
        try
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
            TestLog.Logger.Debug("SafeClick executed via JavaScript.");
        }
        catch (Exception ex)
        {
            TestLog.Logger.Error(ex, "JavaScript click failed");
            element.Click();
            TestLog.Logger.Warning("Fallback to native Click() executed.");
        }
    }
}
