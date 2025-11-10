using EhuTests.NUnit.Core;
using EhuTests.NUnit.Pages.Components;
using OpenQA.Selenium;

namespace EhuTests.NUnit.Pages;

public class HomePage(IWebDriver driver, string baseUrl) : BasePage(driver)
{
    public Header Header => new(Driver);

    public HomePage Open()
    {
        Driver.Navigate().GoToUrl(baseUrl);
        return this;
    }
}
