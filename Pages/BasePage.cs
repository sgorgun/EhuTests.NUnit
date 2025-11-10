using OpenQA.Selenium;
using EhuTests.NUnit.Core;

namespace EhuTests.NUnit.Pages;

public abstract class BasePage(IWebDriver driver)
{
    protected readonly IWebDriver Driver = driver;

    protected IWebElement Find(By locator) => Driver.FindElement(locator);
    protected IReadOnlyCollection<IWebElement> FindAll(By locator) => Driver.FindElements(locator);
}
