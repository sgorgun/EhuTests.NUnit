using EhuTests.NUnit.Core;
using OpenQA.Selenium;

namespace EhuTests.NUnit.Pages;

public class SearchResultsPage(IWebDriver driver) : BasePage(driver)
{
    private readonly By _resultsLocator = By.CssSelector("main, #content, article, .search-results");

    public bool ContainsText(string term)
    {
        TestLog.Logger.Debug("Waiting for results containing term {Term}", term);
        Waits.UntilPageContainsText(Driver, term, RuntimeConfig.Settings.Waits.SearchResultsTextTimeoutSec);
        var containers = FindAll(_resultsLocator);
        var found = containers.Any(c => c.Text.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
               Driver.PageSource.Contains(term, StringComparison.OrdinalIgnoreCase);
        TestLog.Logger.Information("Search results contain term {Term}: {Found}", term, found);
        return found;
    }
}
