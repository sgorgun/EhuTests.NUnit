using OpenQA.Selenium;
using EhuTests.NUnit.Core;

namespace EhuTests.NUnit.Pages;

public abstract class BasePage(IWebDriver driver)
{
    protected readonly IWebDriver Driver = driver;

    protected IWebElement Find(By locator)
    {
        TestLog.Logger.Debug("Finding element by {Locator}", locator);
        return Driver.FindElement(locator);
    }

    protected IReadOnlyCollection<IWebElement> FindAll(By locator)
    {
        TestLog.Logger.Debug("Finding all elements by {Locator}", locator);
        return Driver.FindElements(locator);
    }
}
