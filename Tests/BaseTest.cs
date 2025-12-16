using NUnit.Framework;
using NUnit.Framework.Interfaces;
using EhuTests.NUnit.Core;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using Serilog;
using AventStack.ExtentReports;

namespace EhuTests.NUnit.Tests;

[Parallelizable(ParallelScope.All)]
public abstract class BaseTest
{
    private static TestSettings Settings = default!;
    private DateTime _testStartUtc;
    private ExtentTest? _currentTest;

    protected static TestSettings TestSettings => Settings;

    [OneTimeSetUp]
    public void LoadConfig()
    {
        ReportAggregator.SuiteStarted();

        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.test.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        Settings = config.GetSection("TestSettings").Get<TestSettings>()
            ?? throw new InvalidOperationException("Missing TestSettings configuration.");
        RuntimeConfig.Initialize(Settings);
        TestLog.Initialize(Settings);
        
        // Initialize ExtentReports
        var reportDir = Path.Combine(AppContext.BaseDirectory, "reports");
        var reportPath = Path.Combine(reportDir, $"TestReport_{DateTime.Now:yyyyMMdd_H Hmmss}.html");
        ExtentManager.Initialize(reportPath, "EHU Test Execution Report");
        
        TestLog.Logger.Information("Configuration loaded.");
    }

    [SetUp]
    public void SetUp()
    {
        _testStartUtc = DateTime.UtcNow;
        var testName = TestContext.CurrentContext.Test.Name;
        var fullName = TestContext.CurrentContext.Test.FullName;
        
        TestLog.Logger.Debug("Test SetUp starting for {TestName}", testName);

        // Create ExtentTest for this test
        _currentTest = ExtentTestContext.CreateTest(testName, fullName);
        _currentTest.Info($"Test started at {_testStartUtc:yyyy-MM-dd HH:mm:ss}");
        
        // Add categories to the report
        var categories = TestContext.CurrentContext.Test.Properties["Category"];
        foreach (var category in categories)
        {
            _currentTest.AssignCategory(category.ToString() ?? "General");
        }

        var builder = OptionsBuilder.ForChrome()
            .Headless(Settings.Headless)
            .WithImplicitWait(Settings.ImplicitWaitSec);

        if (Settings.BrowserArgs != null)
            foreach (var arg in Settings.BrowserArgs)
            {
                builder.WithArg(arg);
                TestLog.Logger.Debug("Added browser arg {Arg}", arg);
            }

        var browser = Enum.TryParse<Browser>(Settings.Browser, true, out var b) ? b : Browser.Chrome;
        DriverManager.Instance.Initialize(browser, builder);
        TestLog.Logger.Information("Driver initialized for {Browser}", browser);
        _currentTest.Info($"Browser initialized: {browser}");

        try
        {
            DriverManager.Instance.Driver.Manage().Cookies.DeleteAllCookies();
            TestLog.Logger.Debug("Cookies cleared.");
            _currentTest.Info("Cookies cleared");
        }
        catch (Exception ex)
        {
            TestLog.Logger.Warning(ex, "Failed to clear cookies.");
            _currentTest.Warning($"Failed to clear cookies: {ex.Message}");
        }
    }

    [TearDown]
    public void TearDown()
    {
        var ctx = TestContext.CurrentContext;
        var status = ctx.Result.Outcome.Status.ToString();
        var testName = ctx.Test.Name;
        var durationMs = (DateTime.UtcNow - _testStartUtc).TotalMilliseconds;
        var message = ctx.Result.Message;
        var stackTrace = ctx.Result.StackTrace;

        // Update ExtentReports based on test outcome
        if (_currentTest != null)
        {
            _currentTest.Info($"Test finished at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
            _currentTest.Info($"Execution time: {durationMs:F2} ms");

            switch (ctx.Result.Outcome.Status)
            {
                case TestStatus.Passed:
                    _currentTest.Pass("Test passed successfully");
                    break;
                case TestStatus.Failed:
                    var errorMsg = !string.IsNullOrEmpty(message) ? message : "Test failed";
                    _currentTest.Fail(errorMsg);
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        _currentTest.Fail($"<pre>{stackTrace}</pre>");
                    }
                    break;
                case TestStatus.Skipped:
                case TestStatus.Inconclusive:
                    _currentTest.Skip(!string.IsNullOrEmpty(message) ? message : "Test skipped");
                    break;
                case TestStatus.Warning:
                    _currentTest.Warning(!string.IsNullOrEmpty(message) ? message : "Test completed with warning");
                    break;
            }

            // Try to capture screenshot on failure
            if (ctx.Result.Outcome.Status == TestStatus.Failed)
            {
                try
                {
                    var screenshot = ((ITakesScreenshot)DriverManager.Instance.Driver).GetScreenshot();
                    var screenshotDir = Path.Combine(AppContext.BaseDirectory, "reports", "screenshots");
                    Directory.CreateDirectory(screenshotDir);
                    var screenshotPath = Path.Combine(screenshotDir, $"{testName}_{DateTime.Now:yyyyMMdd_H Hmmss}.png");
                    screenshot.SaveAsFile(screenshotPath);
                    _currentTest.AddScreenCaptureFromPath(screenshotPath, "Failure Screenshot");
                    TestLog.Logger.Information("Screenshot captured: {Path}", screenshotPath);
                }
                catch (Exception ex)
                {
                    TestLog.Logger.Warning(ex, "Failed to capture screenshot");
                }
            }
        }

        // Add to aggregator
        ReportAggregator.Add(testName, status, durationMs, message);

        if (ctx.Result.FailCount > 0)
        {
            TestLog.Logger.Error("Test {TestName} failed. Status={Status} Message={Message}",
                testName, ctx.Result.Outcome.Status, ctx.Result.Message);
        }
        else
        {
            TestLog.Logger.Information("Test {TestName} finished successfully.", testName);
        }

        DriverManager.Instance.Quit();
        TestLog.Logger.Debug("Driver quit.");
        
        // Remove test context
        ExtentTestContext.RemoveTest();
    }

    [OneTimeTearDown]
    public void GlobalTearDown()
    {
        var outDir = Path.Combine(AppContext.BaseDirectory, "reports");
        
        // Write JSON summary
        ReportAggregator.WriteSummary(outDir);
        TestLog.Logger.Information("Summary report written to {Dir}", outDir);

        // Flush ExtentReports
        ExtentManager.Flush();
        TestLog.Logger.Information("ExtentReports HTML report generated.");

        TestLog.Logger.Information("All tests finished.");
        TestLog.Shutdown();
        
        // Shutdown ExtentManager
        ExtentManager.Shutdown();
    }
}
