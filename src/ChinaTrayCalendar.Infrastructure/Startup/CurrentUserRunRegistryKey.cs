using System.Runtime.Versioning;
using Microsoft.Win32;

namespace ChinaTrayCalendar.Infrastructure.Startup;

[SupportedOSPlatform("windows")]
internal sealed class CurrentUserRunRegistryKey : IRunRegistryKey
{
    private const string RunSubKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

    public string? GetValue(string valueName)
    {
        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(RunSubKeyPath, writable: false);

        return key?.GetValue(valueName) as string;
    }

    public void SetValue(string valueName, string value)
    {
        using RegistryKey key = Registry.CurrentUser.CreateSubKey(RunSubKeyPath, writable: true)
            ?? throw new AutoStartRegistrationException(
                "Current-user Run registry key could not be opened.",
                new InvalidOperationException(RunSubKeyPath));

        key.SetValue(valueName, value, RegistryValueKind.String);
    }

    public void DeleteValue(string valueName)
    {
        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(RunSubKeyPath, writable: true);
        key?.DeleteValue(valueName, throwOnMissingValue: false);
    }
}
