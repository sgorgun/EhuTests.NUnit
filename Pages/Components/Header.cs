using OpenQA.Selenium;
using EhuTests.NUnit.Core;

namespace EhuTests.NUnit.Pages.Components;

public class Header(IWebDriver driver)
{
    private readonly IWebDriver _driver = driver;

    private static By AboutLink => By.LinkText("About");
    private static By LtLanguageLink => By.CssSelector("a[href*='lt.ehuniversity.lt']");
    private static By SearchInput => By.CssSelector("input[type='search']");

    public void OpenAbout()
    {
        TestLog.Logger.Debug("Attempting to open About page.");
        var about = _driver.FindElements(AboutLink).FirstOrDefault();
        if (about == null)
        {
            TestLog.Logger.Warning("About link not found.");
            return;
        }
        about.SafeClick(_driver);
        TestLog.Logger.Information("About link clicked.");
    }

    public void SwitchToLt()
    {
        TestLog.Logger.Debug("Attempting to switch to LT language.");
        var lt = _driver.FindElements(LtLanguageLink).FirstOrDefault();
        if (lt == null)
        {
            TestLog.Logger.Warning("LT language link not found.");
            return;
        }
        lt.SafeClick(_driver);
        TestLog.Logger.Information("LT language link clicked.");
    }

    public void Search(string term)
    {
        TestLog.Logger.Debug("Starting search for term {Term}", term);
        var input = _driver.FindElements(SearchInput).FirstOrDefault();
        if (input != null && input.Displayed)
        {
            input.Clear();
            input.SendKeys(term + Keys.Enter);
            TestLog.Logger.Information("Search submitted via input for term {Term}", term);
        }
        else
        {
            TestLog.Logger.Warning("Search input not available, constructing URL.");
            var template = RuntimeConfig.Settings.Urls.SearchTemplate;
            var url = template.Replace("{base}", RuntimeConfig.Settings.Urls.BaseEn)
                .Replace("{term}", System.Uri.EscapeDataString(term));
            _driver.Navigate().GoToUrl(url);
            TestLog.Logger.Information("Navigated directly to search URL {Url}", url);
        }
    }
}
