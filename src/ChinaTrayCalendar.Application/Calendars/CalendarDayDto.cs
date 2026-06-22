using ChinaTrayCalendar.Domain;

namespace ChinaTrayCalendar.Application.Calendars;

public sealed record CalendarDayDto
{
    public CalendarDayDto(
        DateOnly date,
        bool isInCurrentMonth,
        bool isToday,
        bool isWeekend,
        DayMarker marker,
        string? holidayName = null)
    {
        if (!Enum.IsDefined(marker))
        {
            throw new ArgumentOutOfRangeException(nameof(marker), marker, "Day marker is not supported.");
        }

        if (holidayName is not null && string.IsNullOrWhiteSpace(holidayName))
        {
            throw new ArgumentException("Holiday name must not be blank when provided.", nameof(holidayName));
        }

        Date = date;
        IsInCurrentMonth = isInCurrentMonth;
        IsToday = isToday;
        IsWeekend = isWeekend;
        Marker = marker;
        HolidayName = holidayName;
    }

    public DateOnly Date { get; }

    public bool IsInCurrentMonth { get; }

    public bool IsToday { get; }

    public bool IsWeekend { get; }

    public DayMarker Marker { get; }

    public string? HolidayName { get; }
}
