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
        var about = _driver.FindElements(AboutLink).FirstOrDefault();
        about?.SafeClick(_driver);
    }

    public void SwitchToLt()
    {
        var lt = _driver.FindElements(LtLanguageLink).FirstOrDefault();
        lt?.SafeClick(_driver);
    }

    public void Search(string term)
    {
        var input = _driver.FindElements(SearchInput).FirstOrDefault();
        if (input != null && input.Displayed)
        {
            input.Clear();
            input.SendKeys(term + Keys.Enter);
        }
        else
        {
            var template = RuntimeConfig.Settings.Urls.SearchTemplate;
            var url = template.Replace("{base}", RuntimeConfig.Settings.Urls.BaseEn)
                .Replace("{term}", System.Uri.EscapeDataString(term));
            _driver.Navigate().GoToUrl(url);
        }
    }
}
