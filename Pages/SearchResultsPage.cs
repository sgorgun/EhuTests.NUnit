using EhuTests.NUnit.Core;
using OpenQA.Selenium;

namespace EhuTests.NUnit.Pages;

public class SearchResultsPage(IWebDriver driver) : BasePage(driver)
{
    private readonly By _resultsLocator = By.CssSelector("main, #content, article, .search-results");

    public bool ContainsText(string term)
    {
        Waits.UntilPageContainsText(Driver, term, RuntimeConfig.Settings.Waits.SearchResultsTextTimeoutSec);
        var containers = FindAll(_resultsLocator);
        return containers.Any(c => c.Text.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
               Driver.PageSource.Contains(term, StringComparison.OrdinalIgnoreCase);
    }
}
