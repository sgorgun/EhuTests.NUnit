using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace EhuTests.NUnit.Core;

public static class ExtentManager
{
    private static ExtentReports? _extent;
    private static readonly object _lock = new();
    private static bool _initialized;

    public static ExtentReports Instance
    {
        get
        {
            if (_extent == null)
            {
                lock (_lock)
                {
                    if (_extent == null)
                    {
                        throw new InvalidOperationException("ExtentManager not initialized. Call Initialize() first.");
                    }
                }
            }
            return _extent;
        }
    }

    public static void Initialize(string reportPath, string reportName = "Test Execution Report")
    {
        if (_initialized) return;

        lock (_lock)
        {
            if (_initialized) return;

            var reportDir = Path.GetDirectoryName(reportPath);
            if (!string.IsNullOrEmpty(reportDir))
            {
                Directory.CreateDirectory(reportDir);
            }

            var htmlReporter = new ExtentSparkReporter(reportPath);
            
            // Configure the HTML report
            htmlReporter.Config.DocumentTitle = reportName;
            htmlReporter.Config.ReportName = reportName;
            htmlReporter.Config.TimeStampFormat = "yyyy-MM-dd HH:mm:ss";
            htmlReporter.Config.Encoding = "UTF-8";

            _extent = new ExtentReports();
            _extent.AttachReporter(htmlReporter);
            
            // Add system information
            _extent.AddSystemInfo("Environment", "Test");
            _extent.AddSystemInfo("User", Environment.UserName);
            _extent.AddSystemInfo("Machine", Environment.MachineName);
            _extent.AddSystemInfo("OS", Environment.OSVersion.ToString());
            _extent.AddSystemInfo(".NET Version", Environment.Version.ToString());

            _initialized = true;
            TestLog.Logger.Information("ExtentReports initialized. Report will be saved to: {ReportPath}", reportPath);
        }
    }

    public static void Flush()
    {
        if (!_initialized) return;

        lock (_lock)
        {
            if (_extent != null)
            {
                _extent.Flush();
                TestLog.Logger.Information("ExtentReports flushed.");
            }
        }
    }

    public static void Shutdown()
    {
        if (!_initialized) return;

        lock (_lock)
        {
            Flush();
            _initialized = false;
            _extent = null;
            TestLog.Logger.Information("ExtentReports shutdown complete.");
        }
    }
}
