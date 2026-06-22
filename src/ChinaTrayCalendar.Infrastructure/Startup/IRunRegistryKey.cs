namespace ChinaTrayCalendar.Infrastructure.Startup;

internal interface IRunRegistryKey
{
    string? GetValue(string valueName);

    void SetValue(string valueName, string value);

    void DeleteValue(string valueName);
}
