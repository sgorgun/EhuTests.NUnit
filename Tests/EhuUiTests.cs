using EhuTests.NUnit.Core;
using EhuTests.NUnit.Pages;
using EhuTests.NUnit.TestData;
using NUnit.Framework;
using OpenQA.Selenium;
using Shouldly;

namespace EhuTests.NUnit.Tests;

[TestFixture]
public class EhuUiTests : BaseTest
{
    private static IWebDriver Driver => DriverManager.Instance.Driver;

    [Test, Category("Navigation"), RetryOnFailure(2)]
    public void AboutPage_Should_Open()
    {
        TestLog.Logger.Information("Starting About page navigation test.");
        var home = new HomePage(Driver, RuntimeConfig.Settings.Urls.BaseEn).Open();
        home.Header.OpenAbout();
        TestLog.Logger.Debug("Navigated to {Url}", Driver.Url);
        Driver.Url.ToLower().ShouldContain("/about");
        TestLog.Logger.Information("About page assertion passed.");
    }

    [Test, Category("Localization"), RetryOnFailure(2)]
    public void Language_Should_Switch_To_LT()
    {
        TestLog.Logger.Information("Starting language switch test.");
        var home = new HomePage(Driver, RuntimeConfig.Settings.Urls.BaseEn).Open();
        home.Header.SwitchToLt();
        TestLog.Logger.Debug("Current URL after switch {Url}", Driver.Url);
        Driver.Url.ShouldStartWith(RuntimeConfig.Settings.Urls.BaseLt);
        TestLog.Logger.Information("Language switch assertion passed.");
    }

    [TestCaseSource(typeof(SearchData), nameof(SearchData.Terms)), Category("Search"), RetryOnFailure(3)]
    public void Search_Should_Return_Results(string term)
    {
        TestLog.Logger.Information("Starting search test for term {Term}", term);
        var home = new HomePage(Driver, RuntimeConfig.Settings.Urls.BaseEn).Open();
        home.Header.Search(term);
        TestLog.Logger.Debug("Search performed, current URL {Url}", Driver.Url);
        var results = new SearchResultsPage(Driver);
        var keyword = term.Split(' ')[0].ToLower();
        var contains = results.ContainsText(keyword);
        TestLog.Logger.Debug("Results contain keyword {Keyword}: {Contains}", keyword, contains);
        contains.ShouldBeTrue($"Expected search results to contain '{term}'.");
        TestLog.Logger.Information("Search assertion passed for {Term}", term);
    }
}
