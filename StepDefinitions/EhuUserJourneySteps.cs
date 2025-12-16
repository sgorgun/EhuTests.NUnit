using EhuTests.NUnit.Core;
using EhuTests.NUnit.Pages;
using NUnit.Framework;
using OpenQA.Selenium;
using Reqnroll;
using Serilog;
using Shouldly;

namespace EhuTests.NUnit.StepDefinitions;

[Binding]
public class EhuUserJourneySteps
{
    private readonly ScenarioContext _scenarioContext;
    private static IWebDriver Driver => DriverManager.Instance.Driver;

    public EhuUserJourneySteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;

        if (Log.Logger == null)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/bdd-ehu-journey-.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }

    [BeforeScenario]
    public void BeforeScenario()
    {
        Log.Information("BDD scenario start: {Scenario}", _scenarioContext.ScenarioInfo.Title);
    }

    [AfterScenario]
    public void AfterScenario()
    {
        Log.Information("BDD scenario end: {Scenario}", _scenarioContext.ScenarioInfo.Title);
    }

    [Given("I am on the EHU home page")]
    public void GivenIAmOnTheEhuHomePage()
    {
        var baseUrl = RuntimeConfig.Settings.Urls.BaseEn;
        Log.Information("Opening EHU home page: {Url}", baseUrl);
        var home = new HomePage(Driver, baseUrl).Open();
        Driver.Url.ShouldStartWith(baseUrl);
        Log.Information("Home page opened successfully at {Url}", Driver.Url);
        _scenarioContext["HomePage"] = home;
    }

    [When("I open the About page from the header")]
    public void WhenIOpenTheAboutPageFromTheHeader()
    {
        var home = (HomePage)_scenarioContext["HomePage"];
        Log.Information("Opening About page from header.");
        home.Header.OpenAbout();
    }

    [Then("the About page should be displayed")]
    public void ThenTheAboutPageShouldBeDisplayed()
    {
        var currentUrl = Driver.Url.ToLowerInvariant();
        Log.Information("Verifying About page URL: {Url}", currentUrl);
        currentUrl.ShouldContain("/about");
    }

    [When("I switch the language to Lithuanian")]
    public void WhenISwitchTheLanguageToLithuanian()
    {
        var home = (HomePage)_scenarioContext["HomePage"];
        Log.Information("Switching language to LT.");
        home.Header.SwitchToLt();
    }

    [Then("the Lithuanian home page should be displayed")]
    public void ThenTheLithuanianHomePageShouldBeDisplayed()
    {
        var expectedLt = RuntimeConfig.Settings.Urls.BaseLt;
        var currentUrl = Driver.Url;
        Log.Information("Verifying Lithuanian base URL. Expected {Expected}, Actual {Actual}", expectedLt, currentUrl);
        currentUrl.ShouldStartWith(expectedLt);
    }

    [When("I search for \"(.*)\"")]
    public void WhenISearchFor(string term)
    {
        var home = (HomePage)_scenarioContext["HomePage"];
        Log.Information("Searching for term: {Term}", term);
        home.Header.Search(term);
        Log.Information("Search results URL: {Url}", Driver.Url);
        _scenarioContext["SearchTerm"] = term;
    }

    [Then("the search results should contain \"(.*)\"")]
    public void ThenTheSearchResultsShouldContain(string expectedKeyword)
    {
        var results = new SearchResultsPage(Driver);
        var keyword = expectedKeyword.ToLowerInvariant();
        Log.Information("Checking search results contain keyword: {Keyword}", keyword);

        var contains = results.ContainsText(keyword);
        contains.ShouldBeTrue($"Expected search results to contain '{keyword}'.");
    }
}
