using ChinaTrayCalendar.Infrastructure.Startup;

namespace ChinaTrayCalendar.Infrastructure.Tests;

public sealed class WindowsAutoStartServiceTests
{
    private const string ExecutablePath = @"C:\Program Files\Dateview\ChinaTrayCalendar.Desktop.exe";
    private const string QuotedExecutablePath = "\"C:\\Program Files\\Dateview\\ChinaTrayCalendar.Desktop.exe\"";

    [Fact]
    public void ConstructorRejectsBlankPath()
    {
        FakeRunRegistryKey registryKey = new();

        Assert.Throws<ArgumentException>(() => new WindowsAutoStartService("   ", registryKey));
    }

    [Fact]
    public void ConstructorRejectsRelativePath()
    {
        FakeRunRegistryKey registryKey = new();

        Assert.Throws<ArgumentException>(() => new WindowsAutoStartService(@"Dateview.exe", registryKey));
    }

    [Fact]
    public void ConstructorRejectsPathWithQuote()
    {
        FakeRunRegistryKey registryKey = new();

        Assert.Throws<ArgumentException>(() => new WindowsAutoStartService(
            "C:\\Program Files\\Dateview\\Date\"view.exe",
            registryKey));
    }

    [Fact]
    public void SetEnabledWritesQuotedExecutablePath()
    {
        FakeRunRegistryKey registryKey = new();
        WindowsAutoStartService service = new(ExecutablePath, registryKey);

        service.SetEnabled(enabled: true);

        Assert.Equal(QuotedExecutablePath, registryKey.Values[WindowsAutoStartService.RunValueName]);
    }

    [Fact]
    public void IsEnabledReturnsTrueWhenRunValueMatches()
    {
        FakeRunRegistryKey registryKey = new();
        registryKey.Values[WindowsAutoStartService.RunValueName] = QuotedExecutablePath;
        WindowsAutoStartService service = new(ExecutablePath, registryKey);

        Assert.True(service.IsEnabled());
    }

    [Fact]
    public void IsEnabledReturnsFalseWhenRunValueIsMissing()
    {
        FakeRunRegistryKey registryKey = new();
        WindowsAutoStartService service = new(ExecutablePath, registryKey);

        Assert.False(service.IsEnabled());
    }

    [Fact]
    public void IsEnabledReturnsFalseWhenRunValueDiffers()
    {
        FakeRunRegistryKey registryKey = new();
        registryKey.Values[WindowsAutoStartService.RunValueName] = "\"C:\\Other\\Dateview.exe\"";
        WindowsAutoStartService service = new(ExecutablePath, registryKey);

        Assert.False(service.IsEnabled());
    }

    [Fact]
    public void SetEnabledFalseDeletesRunValue()
    {
        FakeRunRegistryKey registryKey = new();
        registryKey.Values[WindowsAutoStartService.RunValueName] = QuotedExecutablePath;
        WindowsAutoStartService service = new(ExecutablePath, registryKey);

        service.SetEnabled(enabled: false);

        Assert.False(registryKey.Values.ContainsKey(WindowsAutoStartService.RunValueName));
    }

    [Fact]
    public void IsEnabledWrapsRegistryFailure()
    {
        ThrowingRunRegistryKey registryKey = new();
        WindowsAutoStartService service = new(ExecutablePath, registryKey);

        AutoStartRegistrationException exception = Assert.Throws<AutoStartRegistrationException>(
            () => service.IsEnabled());

        Assert.Contains("read", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SetEnabledWrapsRegistryFailure()
    {
        ThrowingRunRegistryKey registryKey = new();
        WindowsAutoStartService service = new(ExecutablePath, registryKey);

        AutoStartRegistrationException exception = Assert.Throws<AutoStartRegistrationException>(
            () => service.SetEnabled(enabled: true));

        Assert.Contains("updated", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class FakeRunRegistryKey : IRunRegistryKey
    {
        public Dictionary<string, string> Values { get; } = [];

        public string? GetValue(string valueName)
        {
            return Values.TryGetValue(valueName, out string? value) ? value : null;
        }

        public void SetValue(string valueName, string value)
        {
            Values[valueName] = value;
        }

        public void DeleteValue(string valueName)
        {
            Values.Remove(valueName);
        }
    }

    private sealed class ThrowingRunRegistryKey : IRunRegistryKey
    {
        public string? GetValue(string valueName)
        {
            throw new UnauthorizedAccessException(valueName);
        }

        public void SetValue(string valueName, string value)
        {
            throw new UnauthorizedAccessException(valueName);
        }

        public void DeleteValue(string valueName)
        {
            throw new UnauthorizedAccessException(valueName);
        }
    }
}
