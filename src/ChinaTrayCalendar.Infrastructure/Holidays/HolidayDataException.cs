namespace ChinaTrayCalendar.Infrastructure.Holidays;

public sealed class HolidayDataException : Exception
{
    public HolidayDataException(string message)
        : base(message)
    {
    }

    public HolidayDataException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
