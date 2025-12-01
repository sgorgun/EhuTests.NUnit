using AventStack.ExtentReports;

namespace EhuTests.NUnit.Core;

/// <summary>
/// Helper class for logging to ExtentReports from test methods.
/// Provides convenient methods for adding logs to the current test.
/// </summary>
public static class ExtentTestLogger
{
    /// <summary>
    /// Logs an informational message to the current test.
    /// </summary>
    /// <param name="message">The message to log</param>
    public static void Info(string message)
    {
        ExtentTestContext.CurrentTest?.Info(message);
        TestLog.Logger.Information(message);
    }

    /// <summary>
    /// Logs a warning message to the current test.
    /// </summary>
    /// <param name="message">The warning message</param>
    public static void Warning(string message)
    {
        ExtentTestContext.CurrentTest?.Warning(message);
        TestLog.Logger.Warning(message);
    }

    /// <summary>
    /// Logs a step execution to the current test.
    /// </summary>
    /// <param name="stepDescription">Description of the step being performed</param>
    public static void Step(string stepDescription)
    {
        ExtentTestContext.CurrentTest?.Info($"<b>Step:</b> {stepDescription}");
        TestLog.Logger.Information("Step: {Step}", stepDescription);
    }

    /// <summary>
    /// Logs a pass message to the current test.
    /// </summary>
    /// <param name="message">The pass message</param>
    public static void Pass(string message)
    {
        ExtentTestContext.CurrentTest?.Pass(message);
        TestLog.Logger.Information("? {Message}", message);
    }

    /// <summary>
    /// Logs navigation to a URL.
    /// </summary>
    /// <param name="url">The URL being navigated to</param>
    public static void NavigatedTo(string url)
    {
        ExtentTestContext.CurrentTest?.Info($"Navigated to: <a href='{url}' target='_blank'>{url}</a>");
        TestLog.Logger.Information("Navigated to {Url}", url);
    }

    /// <summary>
    /// Logs an assertion being verified.
    /// </summary>
    /// <param name="assertion">Description of the assertion</param>
    /// <param name="passed">Whether the assertion passed</param>
    public static void Assert(string assertion, bool passed = true)
    {
        var status = passed ? "?" : "?";
        var message = $"{status} Assert: {assertion}";
        
        if (passed)
        {
            ExtentTestContext.CurrentTest?.Pass(message);
            TestLog.Logger.Information(message);
        }
        else
        {
            ExtentTestContext.CurrentTest?.Fail(message);
            TestLog.Logger.Error(message);
        }
    }

    /// <summary>
    /// Logs test data being used.
    /// </summary>
    /// <param name="dataName">Name of the test data</param>
    /// <param name="dataValue">Value of the test data</param>
    public static void TestData(string dataName, object dataValue)
    {
        ExtentTestContext.CurrentTest?.Info($"<b>{dataName}:</b> {dataValue}");
        TestLog.Logger.Information("{DataName}: {DataValue}", dataName, dataValue);
    }

    /// <summary>
    /// Adds a screenshot to the current test.
    /// </summary>
    /// <param name="screenshotPath">Path to the screenshot file</param>
    /// <param name="title">Title/description of the screenshot</param>
    public static void AddScreenshot(string screenshotPath, string title = "Screenshot")
    {
        if (File.Exists(screenshotPath))
        {
            ExtentTestContext.CurrentTest?.AddScreenCaptureFromPath(screenshotPath, title);
            TestLog.Logger.Information("Screenshot added: {Path}", screenshotPath);
        }
        else
        {
            TestLog.Logger.Warning("Screenshot file not found: {Path}", screenshotPath);
        }
    }
}
