namespace ChinaTrayCalendar.Domain;

public sealed class MonthGridBuilder
{
    public MonthGrid Build(CalendarMonth month)
    {
        return Build(month, DayOfWeek.Monday);
    }

    public MonthGrid Build(CalendarMonth month, DayOfWeek firstDayOfWeek)
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
            .Select(offset => CreateDay(firstVisibleDate.AddDays(offset), month))
            .ToArray();

        return new MonthGrid(month, days);
    }

    private static CalendarDay CreateDay(DateOnly date, CalendarMonth month)
    {
        return new CalendarDay(
            date,
            isInCurrentMonth: date.Year == month.Year && date.Month == month.Month);
    }

    private static DateOnly GetFirstVisibleDate(DateOnly firstOfMonth, DayOfWeek firstDayOfWeek)
    {
        int daysSinceFirstDayOfWeek = ((int)firstOfMonth.DayOfWeek - (int)firstDayOfWeek + 7) % 7;

        return firstOfMonth.AddDays(-daysSinceFirstDayOfWeek);
    }
}
