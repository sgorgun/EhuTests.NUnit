using OpenQA.Selenium.Chrome;
using Serilog;

namespace EhuTests.NUnit.Core;

public sealed class OptionsBuilder
{
    private bool _headless;
    private int? _implicitWaitSec;
    private readonly List<string> _args = [];

    public static OptionsBuilder ForChrome()
    {
        TestLog.Logger.Debug("Creating Chrome OptionsBuilder");
        return new();
    }

    public OptionsBuilder Headless(bool enabled = true)
    {
        _headless = enabled;
        TestLog.Logger.Debug("Set headless={Headless}", enabled);
        return this;
    }

    public OptionsBuilder WithArg(string arg)
    {
        _args.Add(arg);
        TestLog.Logger.Debug("Added Chrome arg {Arg}", arg);
        return this;
    }

    public OptionsBuilder WithImplicitWait(int seconds)
    {
        _implicitWaitSec = seconds;
        TestLog.Logger.Debug("Set implicit wait={Seconds}s", seconds);
        return this;
    }

    public ChromeOptions BuildChromeOptions()
    {
        TestLog.Logger.Debug("Building ChromeOptions (Headless={Headless}, ArgsCount={Count})", _headless, _args.Count);
        var opts = new ChromeOptions();
        if (_headless) opts.AddArgument("--headless=new");
        foreach (var arg in _args) opts.AddArgument(arg);
        return opts;
    }

    public int? ImplicitWaitSeconds() => _implicitWaitSec;
}
