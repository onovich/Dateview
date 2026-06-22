namespace ChinaTrayCalendar.Infrastructure.Settings;

public sealed class SettingsDataException : Exception
{
    public SettingsDataException(string message)
        : base(message)
    {
    }

    public SettingsDataException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
