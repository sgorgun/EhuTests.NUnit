using EhuTests.NUnit.Core;
using EhuTests.NUnit.Pages.Components;
using OpenQA.Selenium;

namespace EhuTests.NUnit.Pages;

public class HomePage(IWebDriver driver, string baseUrl) : BasePage(driver)
{
    private readonly string _baseUrl = baseUrl;
    public Header Header => new(Driver);

    public HomePage Open()
    {
        TestLog.Logger.Information("Opening home page {Url}", _baseUrl);
        Driver.Navigate().GoToUrl(_baseUrl);
        return this;
    }
}
