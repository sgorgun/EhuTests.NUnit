using System.Threading;
using OpenQA.Selenium;
using Serilog;

namespace EhuTests.NUnit.Core
{
    public sealed class DriverManager
    {
        private static readonly Lazy<DriverManager> _lazy = new(() => new DriverManager());
        public static DriverManager Instance => _lazy.Value;

        private readonly ThreadLocal<IWebDriver?> _driver = new(() => null, true);

        private DriverManager() { }

        public IWebDriver Driver => _driver.Value ?? throw new InvalidOperationException("Driver not initialized.");

        public void Initialize(Browser browser, OptionsBuilder builder)
        {
            if (_driver.Value != null) return;
            TestLog.Logger.Information("Initializing WebDriver for {Browser}", browser);
            _driver.Value = WebDriverFactory.Create(browser, builder);
        }

        public void Quit()
        {
            try
            {
                _driver.Value?.Quit();
                _driver.Value?.Dispose();
                TestLog.Logger.Information("WebDriver quit and disposed.");
            }
            catch (Exception ex)
            {
                TestLog.Logger.Error(ex, "Error while quitting WebDriver");
            }
            finally
            {
                _driver.Value = null;
            }
        }
    }
}
