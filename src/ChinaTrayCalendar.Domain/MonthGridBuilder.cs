namespace ChinaTrayCalendar.Domain;

public sealed class MonthGridBuilder
{
    public MonthGrid Build(CalendarMonth month)
    {
        return Build(month, today: null, DayOfWeek.Monday);
    }

    public MonthGrid Build(CalendarMonth month, DayOfWeek firstDayOfWeek)
    {
        return Build(month, today: null, firstDayOfWeek);
    }

    public MonthGrid Build(CalendarMonth month, DateOnly today)
    {
        return Build(month, today, DayOfWeek.Monday);
    }

    public MonthGrid Build(CalendarMonth month, DateOnly today, DayOfWeek firstDayOfWeek)
    {
        return Build(month, (DateOnly?)today, firstDayOfWeek);
    }

    private MonthGrid Build(CalendarMonth month, DateOnly? today, DayOfWeek firstDayOfWeek)
    {
        if (!Enum.IsDefined(firstDayOfWeek))
        {
            throw new ArgumentOutOfRangeException(
                nameof(firstDayOfWeek),
                firstDayOfWeek,
                "First day of week is not supported.");
        }

        DateOnly firstVisibleDate = GetFirstVisibleDate(month.FirstDay, firstDayOfWeek);
        CalendarDay[] days = Enumerable
            .Range(0, MonthGrid.CellCount)
            .Select(offset => CreateDay(firstVisibleDate.AddDays(offset), month, today))
            .ToArray();

        return new MonthGrid(month, days);
    }

    private static CalendarDay CreateDay(DateOnly date, CalendarMonth month, DateOnly? today)
    {
        return new CalendarDay(
            date,
            isInCurrentMonth: date.Year == month.Year && date.Month == month.Month,
            isToday: date == today,
            isWeekend: date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday);
    }

    private static DateOnly GetFirstVisibleDate(DateOnly firstOfMonth, DayOfWeek firstDayOfWeek)
    {
        int daysSinceFirstDayOfWeek = ((int)firstOfMonth.DayOfWeek - (int)firstDayOfWeek + 7) % 7;

        return firstOfMonth.AddDays(-daysSinceFirstDayOfWeek);
    }
}
