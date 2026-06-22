namespace ChinaTrayCalendar.Infrastructure.Startup;

public sealed class AutoStartRegistrationException : Exception
{
    public AutoStartRegistrationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
