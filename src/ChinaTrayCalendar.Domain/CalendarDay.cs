namespace ChinaTrayCalendar.Domain;

public sealed record CalendarDay
{
    public CalendarDay(
        DateOnly date,
        bool isInCurrentMonth,
        bool isToday = false,
        bool isWeekend = false,
        DayMarker marker = DayMarker.None)
    {
        if (!Enum.IsDefined(marker))
        {
            throw new ArgumentOutOfRangeException(nameof(marker), marker, "Day marker is not supported.");
        }

        Date = date;
        IsInCurrentMonth = isInCurrentMonth;
        IsToday = isToday;
        IsWeekend = isWeekend;
        Marker = marker;
    }

    public DateOnly Date { get; }

    public bool IsInCurrentMonth { get; }

    public bool IsToday { get; }

    public bool IsWeekend { get; }

    public DayMarker Marker { get; }
}
