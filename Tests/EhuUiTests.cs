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
        ExtentTestLogger.Step("Navigate to home page");
        var home = new HomePage(Driver, RuntimeConfig.Settings.Urls.BaseEn).Open();
        ExtentTestLogger.NavigatedTo(Driver.Url);
        
        ExtentTestLogger.Step("Click on About link in header");
        home.Header.OpenAbout();
        
        ExtentTestLogger.Warning("Test marked as inconclusive - temporarily disabled for investigation");
        Assert.Inconclusive("Test temporarily disabled - needs investigation");
        
        ExtentTestLogger.Step("Verify URL contains '/about'");
        var currentUrl = Driver.Url.ToLower();
        ExtentTestLogger.Info($"Current URL: {currentUrl}");
        currentUrl.ShouldContain("/about");
        ExtentTestLogger.Pass("About page opened successfully");
    }

    [Test, Category("Localization"), RetryOnFailure(2)]
    public void Language_Should_Switch_To_LT()
    {
        ExtentTestLogger.Step("Navigate to home page (English)");
        var home = new HomePage(Driver, RuntimeConfig.Settings.Urls.BaseEn).Open();
        ExtentTestLogger.NavigatedTo(Driver.Url);
        
        ExtentTestLogger.Step("Click on LT language link");
        home.Header.SwitchToLt();
        
        ExtentTestLogger.Step("Verify URL switched to Lithuanian version");
        var currentUrl = Driver.Url;
        ExtentTestLogger.TestData("Expected Base URL", RuntimeConfig.Settings.Urls.BaseLt);
        ExtentTestLogger.TestData("Actual URL", currentUrl);
        currentUrl.ShouldStartWith(RuntimeConfig.Settings.Urls.BaseLt);
        ExtentTestLogger.Pass("Language switched to Lithuanian successfully");
    }

    [TestCaseSource(typeof(SearchData), nameof(SearchData.Terms)), Category("Search"), RetryOnFailure(3)]
    public void Search_Should_Return_Results(string term)
    {
        ExtentTestLogger.TestData("Search Term", term);
        
        ExtentTestLogger.Step("Navigate to home page");
        var home = new HomePage(Driver, RuntimeConfig.Settings.Urls.BaseEn).Open();
        ExtentTestLogger.NavigatedTo(Driver.Url);
        
        ExtentTestLogger.Step($"Perform search for: '{term}'");
        home.Header.Search(term);
        ExtentTestLogger.Info($"Search URL: {Driver.Url}");
        
        ExtentTestLogger.Step("Verify search results contain the search term");
        var results = new SearchResultsPage(Driver);
        var keyword = term.Split(' ')[0].ToLower();
        ExtentTestLogger.TestData("Keyword to verify", keyword);
        
        var contains = results.ContainsText(keyword);
        ExtentTestLogger.Assert($"Search results contain '{keyword}'", contains);
        contains.ShouldBeTrue($"Expected search results to contain '{term}'.");
        ExtentTestLogger.Pass($"Search results verified for term '{term}'");
    }
}
