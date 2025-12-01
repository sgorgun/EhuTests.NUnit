using System.Collections.Concurrent;

namespace EhuTests.NUnit.Core;

public static class ReportAggregator
{
    private static readonly ConcurrentBag<(string Name, string Status, double DurationMs)> _results = [];
    private static DateTime _suiteStartUtc = DateTime.UtcNow;
    private static readonly System.Text.Json.JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public static void SuiteStarted()
    {
        _suiteStartUtc = DateTime.UtcNow;
    }

    public static void Add(string name, string status, double durationMs)
    {
        _results.Add((name, status, durationMs));
    }

    public static void WriteSummary(string outputDir)
    {
        Directory.CreateDirectory(outputDir);
        var reportPath = Path.Combine(outputDir, "summary.json");
        var summary = new
        {
            SuiteStartUtc = _suiteStartUtc.ToString("O"),
            SuiteEndUtc = DateTime.UtcNow.ToString("O"),
            Tests = _results.Select(r => new { r.Name, r.Status, r.DurationMs }).ToArray()
        };
        var json = System.Text.Json.JsonSerializer.Serialize(summary, _jsonOptions);
        File.WriteAllText(reportPath, json);
    }
}
