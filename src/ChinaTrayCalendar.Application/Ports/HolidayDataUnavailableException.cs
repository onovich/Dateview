namespace ChinaTrayCalendar.Application.Ports;

public sealed class HolidayDataUnavailableException : Exception
{
    public HolidayDataUnavailableException(int year, string message)
        : base(message)
    {
        Year = year;
    }

    public HolidayDataUnavailableException(int year, string message, Exception innerException)
        : base(message, innerException)
    {
        Year = year;
    }

    public int Year { get; }
}
