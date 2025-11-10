namespace EhuTests.NUnit.Core;

public static class RuntimeConfig
{
    private static TestSettings? _settings; // set from configuration at startup
    private static bool _initialized;

    public static TestSettings Settings => _settings ?? throw new InvalidOperationException("RuntimeConfig not initialized.");

    public static void Initialize(TestSettings settings)
    {
        if (_initialized) return;
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _initialized = true;
    }
}
