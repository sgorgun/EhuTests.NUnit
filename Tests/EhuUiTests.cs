using EhuTests.NUnit.Core;
using EhuTests.NUnit.Pages;
using EhuTests.NUnit.TestData;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using OpenQA.Selenium;

namespace EhuTests.NUnit.Tests;

[TestFixture]
public class EhuUiTests : BaseTest
{
    private static IWebDriver Driver => DriverManager.Instance.Driver;

    [Test, Category("Navigation"), RetryOnFailure(2)]
    public void AboutPage_Should_Open()
    {
        var home = new HomePage(Driver, RuntimeConfig.Settings.Urls.BaseEn).Open();
        home.Header.OpenAbout();
        StringAssert.Contains("/about", Driver.Url.ToLower());
    }

    [Test, Category("Localization"), RetryOnFailure(2)]
    public void Language_Should_Switch_To_LT()
    {
        var home = new HomePage(Driver, RuntimeConfig.Settings.Urls.BaseEn).Open();
        home.Header.SwitchToLt();
        StringAssert.StartsWith(RuntimeConfig.Settings.Urls.BaseLt, Driver.Url);
    }

    [TestCaseSource(typeof(SearchData), nameof(SearchData.Terms)),
     Category("Search"),
     RetryOnFailure(3)]
    public void Search_Should_Return_Results(string term)
    {
        var home = new HomePage(Driver, RuntimeConfig.Settings.Urls.BaseEn).Open();
        home.Header.Search(term);
        var results = new SearchResultsPage(Driver);
        Assert.That(results.ContainsText(term.Split(' ')[0].ToLower()), Is.True,
            $"Expected search results to contain '{term}'.");
    }
}
