using OpenQA.Selenium.Chrome;

namespace EhuTests.NUnit.Core;

public sealed class OptionsBuilder
{
    private bool _headless;
    private int? _implicitWaitSec;
    private readonly List<string> _args = [];

    public static OptionsBuilder ForChrome() => new();

    public OptionsBuilder Headless(bool enabled = true)
    { _headless = enabled; return this; }

    public OptionsBuilder WithArg(string arg)
    { _args.Add(arg); return this; }

    public OptionsBuilder WithImplicitWait(int seconds)
    { _implicitWaitSec = seconds; return this; }

    public ChromeOptions BuildChromeOptions()
    {
        var opts = new ChromeOptions();
        if (_headless) opts.AddArgument("--headless=new");
        foreach (var arg in _args) opts.AddArgument(arg);
        return opts;
    }

    public int? ImplicitWaitSeconds() => _implicitWaitSec;
}
