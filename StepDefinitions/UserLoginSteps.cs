using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Reqnroll;
using Serilog;

namespace EhuTests.NUnit.StepDefinitions
{
    [Binding]
    public class UserLoginSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private IWebDriver? _driver;

        public UserLoginSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;

            if (Log.Logger == null)
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .WriteTo.File("logs/bdd-tests-.log", rollingInterval: RollingInterval.Day)
                    .CreateLogger();
            }
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            Log.Information("Starting scenario: {Scenario}", _scenarioContext.ScenarioInfo.Title);
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
        }

        [AfterScenario]
        public void AfterScenario()
        {
            try
            {
                Log.Information("Finished scenario: {Scenario}", _scenarioContext.ScenarioInfo.Title);
            }
            finally
            {
                _driver?.Quit();
            }
        }

        [Given("the application is running")]
        public void GivenTheApplicationIsRunning()
        {
            Log.Debug("Application running check placeholder.");
        }

        [Given("I am on the login page")]
        public void GivenIAmOnTheLoginPage()
        {
            AssertDriverInitialized();
            var loginUrl = "https://example.com/login"; // TODO: replace with real login URL
            Log.Information("Navigating to login page: {Url}", loginUrl);
            _driver!.Navigate().GoToUrl(loginUrl);
        }

        [When("I enter a valid username and password")]
        public void WhenIEnterAValidUsernameAndPassword()
        {
            AssertDriverInitialized();
            var username = "test.user@example.com"; // TODO: valid username
            var password = "P@ssw0rd!";             // TODO: valid password
            Log.Information("Entering valid credentials.");
            _driver!.FindElement(By.Id("username")).SendKeys(username);
            _driver.FindElement(By.Id("password")).SendKeys(password);
        }

        [When("I enter a valid username and an invalid password")]
        public void WhenIEnterAValidUsernameAndAnInvalidPassword()
        {
            AssertDriverInitialized();
            var username = "test.user@example.com"; // TODO: valid username
            var password = "invalid";               // invalid password
            Log.Information("Entering valid username and invalid password.");
            _driver!.FindElement(By.Id("username")).SendKeys(username);
            _driver.FindElement(By.Id("password")).SendKeys(password);
        }

        [When("I click the login button")]
        public void WhenIClickTheLoginButton()
        {
            AssertDriverInitialized();
            Log.Information("Clicking login button.");
            _driver!.FindElement(By.Id("login-button")).Click();
        }

        [Then("I should be redirected to the dashboard")]
        public void ThenIShouldBeRedirectedToTheDashboard()
        {
            AssertDriverInitialized();
            var expectedUrlFragment = "/dashboard"; // TODO: adjust to your app
            Log.Information("Checking dashboard redirection.");
            Assert.That(_driver!.Url, Does.Contain(expectedUrlFragment), "User is not on dashboard.");
        }

        [Then("I should see my user greeting")]
        public void ThenIShouldSeeMyUserGreeting()
        {
            AssertDriverInitialized();
            var greetingElement = _driver!.FindElement(By.Id("greeting")); // TODO: adjust locator
            var text = greetingElement.Text;
            Log.Information("Greeting text: {Greeting}", text);
            Assert.That(text, Does.Contain("Welcome"), "Greeting is not correct.");
        }

        [Then("I should see an error message saying \"(.*)\"")]
        public void ThenIShouldSeeAnErrorMessageSaying(string expectedMessage)
        {
            AssertDriverInitialized();
            try
            {
                var errorElement = _driver!.FindElement(By.Id("login-error")); // TODO: adjust locator
                var actual = errorElement.Text;
                Log.Information("Error message on page: {Error}", actual);
                Assert.That(actual, Does.Contain(expectedMessage), "Error message does not match.");
            }
            catch (NoSuchElementException ex)
            {
                Log.Error(ex, "Login error element not found.");
                Assert.Fail("Expected error message element with id 'login-error' was not found.");
            }
        }

        [Given("I am logged in as a valid user")]
        public void GivenIAmLoggedInAsAValidUser()
        {
            // Reuse existing steps to avoid duplication
            GivenIAmOnTheLoginPage();
            WhenIEnterAValidUsernameAndPassword();
            WhenIClickTheLoginButton();
            ThenIShouldBeRedirectedToTheDashboard();
        }

        [When("I close the browser")]
        public void WhenI_CLoseTheBrowser()
        {
            AssertDriverInitialized();
            Log.Information("Closing browser in scenario.");
            _driver!.Quit();
            _driver = null;
        }

        [When("I open the browser again")]
        public void WhenIOpenTheBrowserAgain()
        {
            Log.Information("Re-opening browser.");
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
        }

        [When("I navigate to the dashboard page")]
        public void WhenINavigateToTheDashboardPage()
        {
            AssertDriverInitialized();
            var dashboardUrl = "https://example.com/dashboard"; // TODO: replace
            Log.Information("Navigating directly to dashboard: {Url}", dashboardUrl);
            _driver!.Navigate().GoToUrl(dashboardUrl);
        }

        [Then("I should be asked to log in again")]
        public void ThenIShouldBeAskedToLogInAgain()
        {
            AssertDriverInitialized();
            Log.Information("Verifying login is required again.");
            var url = _driver!.Url;
            Assert.That(url, Does.Contain("/login"), "User is not redirected to login.");
            var loginForm = _driver.FindElement(By.Id("login-form")); // TODO: adjust locator
            Assert.That(loginForm.Displayed, "Login form is not displayed.");
        }

        private void AssertDriverInitialized()
        {
            if (_driver == null)
            {
                Log.Error("WebDriver is not initialized.");
                Assert.Fail("WebDriver is not initialized. Check BeforeScenario hook.");
            }
        }
    }
}
