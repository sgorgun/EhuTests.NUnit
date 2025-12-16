using AventStack.ExtentReports;
using System.Collections.Concurrent;
using NUnit.Framework;

namespace EhuTests.NUnit.Core;

/// <summary>
/// Thread-safe test context manager for ExtentReports.
/// Manages ExtentTest instances per test execution.
/// </summary>
public static class ExtentTestContext
{
    private static readonly ConcurrentDictionary<string, ExtentTest> _testContexts = new();

    /// <summary>
    /// Creates and registers a new test in the ExtentReports.
    /// </summary>
    /// <param name="testName">Name of the test</param>
    /// <param name="description">Optional test description</param>
    /// <returns>The created ExtentTest instance</returns>
    public static ExtentTest CreateTest(string testName, string? description = null)
    {
        var testId = GetTestId();
        var test = string.IsNullOrEmpty(description)
            ? ExtentManager.Instance.CreateTest(testName)
            : ExtentManager.Instance.CreateTest(testName, description);

        _testContexts[testId] = test;
        TestLog.Logger.Debug("ExtentTest created for {TestName} with ID {TestId}", testName, testId);
        return test;
    }

    /// <summary>
    /// Gets the current test's ExtentTest instance.
    /// </summary>
    public static ExtentTest? CurrentTest
    {
        get
        {
            var testId = GetTestId();
            _testContexts.TryGetValue(testId, out var test);
            return test;
        }
    }

    /// <summary>
    /// Removes the test context for the current test.
    /// </summary>
    public static void RemoveTest()
    {
        var testId = GetTestId();
        _testContexts.TryRemove(testId, out _);
        TestLog.Logger.Debug("ExtentTest removed for ID {TestId}", testId);
    }

    /// <summary>
    /// Clears all test contexts.
    /// </summary>
    public static void Clear()
    {
        _testContexts.Clear();
        TestLog.Logger.Debug("All ExtentTest contexts cleared.");
    }

    /// <summary>
    /// Gets a unique identifier for the current test execution.
    /// Combines thread ID and test name for parallel test support.
    /// </summary>
    private static string GetTestId()
    {
        var threadId = Environment.CurrentManagedThreadId;
        var testName = TestContext.CurrentContext?.Test?.Name ?? "Unknown";
        return $"{threadId}_{testName}";
    }
}
