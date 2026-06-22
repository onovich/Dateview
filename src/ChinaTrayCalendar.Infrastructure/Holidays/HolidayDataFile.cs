using ChinaTrayCalendar.Domain;

namespace ChinaTrayCalendar.Infrastructure.Holidays;

public sealed class HolidayDataFile
{
    public HolidayDataFile(int year, HolidaySource source, IReadOnlyList<HolidayDay> days)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(days);

        Year = year;
        Source = source;
        Days = days.ToArray();
    }

    public int Year { get; }

    public HolidaySource Source { get; }

    public IReadOnlyList<HolidayDay> Days { get; }
}
