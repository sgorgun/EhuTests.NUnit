using System.Collections.Concurrent;

namespace EhuTests.NUnit.Core;

/// <summary>
/// Aggregates test results for summary reporting.
/// Works alongside ExtentReports to provide additional JSON summary.
/// </summary>
public static class ReportAggregator
{
    private static readonly ConcurrentBag<TestResult> _results = [];
    private static DateTime _suiteStartUtc = DateTime.UtcNow;
    private static readonly System.Text.Json.JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public static void SuiteStarted()
    {
        _suiteStartUtc = DateTime.UtcNow;
        TestLog.Logger.Information("Test suite started at {Timestamp}", _suiteStartUtc.ToString("O"));
    }

    public static void Add(string name, string status, double durationMs, string? message = null)
    {
        var result = new TestResult
        {
            Name = name,
            Status = status,
            DurationMs = durationMs,
            Timestamp = DateTime.UtcNow.ToString("O"),
            Message = message
        };
        _results.Add(result);
        TestLog.Logger.Debug("Test result added: {Name} - {Status} - {Duration}ms", name, status, durationMs);
    }

    public static void WriteSummary(string outputDir)
    {
        Directory.CreateDirectory(outputDir);
        var reportPath = Path.Combine(outputDir, "summary.json");
        
        var suiteEndUtc = DateTime.UtcNow;
        var totalDuration = (suiteEndUtc - _suiteStartUtc).TotalMilliseconds;
        var results = _results.ToArray();
        
        var summary = new
        {
            SuiteStartUtc = _suiteStartUtc.ToString("O"),
            SuiteEndUtc = suiteEndUtc.ToString("O"),
            TotalDurationMs = totalDuration,
            TotalTests = results.Length,
            Passed = results.Count(r => r.Status.Equals("Passed", StringComparison.OrdinalIgnoreCase)),
            Failed = results.Count(r => r.Status.Equals("Failed", StringComparison.OrdinalIgnoreCase)),
            Skipped = results.Count(r => r.Status.Equals("Skipped", StringComparison.OrdinalIgnoreCase) || 
                                          r.Status.Equals("Ignored", StringComparison.OrdinalIgnoreCase)),
            Tests = results.OrderBy(r => r.Name).ToArray()
        };
        
        var json = System.Text.Json.JsonSerializer.Serialize(summary, _jsonOptions);
        File.WriteAllText(reportPath, json);
        
        TestLog.Logger.Information("Summary report written to {Path}. Total: {Total}, Passed: {Passed}, Failed: {Failed}, Skipped: {Skipped}",
            reportPath, summary.TotalTests, summary.Passed, summary.Failed, summary.Skipped);
    }

    public static void Clear()
    {
        _results.Clear();
        TestLog.Logger.Debug("ReportAggregator cleared.");
    }

    private class TestResult
    {
        public string Name { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public double DurationMs { get; init; }
        public string Timestamp { get; init; } = string.Empty;
        public string? Message { get; init; }
    }
}
