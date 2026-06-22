namespace ChinaTrayCalendar.Domain;

public sealed class MonthGridBuilder
{
    public MonthGrid Build(CalendarMonth month)
    {
        return Build(month, today: null, holidays: null, DayOfWeek.Monday);
    }

    public MonthGrid Build(CalendarMonth month, DayOfWeek firstDayOfWeek)
    {
        return Build(month, today: null, holidays: null, firstDayOfWeek);
    }

    public MonthGrid Build(CalendarMonth month, DateOnly today)
    {
        return Build(month, today, DayOfWeek.Monday);
    }

    public MonthGrid Build(CalendarMonth month, DateOnly today, DayOfWeek firstDayOfWeek)
    {
        return Build(month, (DateOnly?)today, holidays: null, firstDayOfWeek);
    }

    public MonthGrid Build(CalendarMonth month, DateOnly today, IReadOnlyCollection<HolidayDay> holidays)
    {
        return Build(month, today, holidays, DayOfWeek.Monday);
    }

    public MonthGrid Build(
        CalendarMonth month,
        DateOnly today,
        IReadOnlyCollection<HolidayDay> holidays,
        DayOfWeek firstDayOfWeek)
    {
        ArgumentNullException.ThrowIfNull(holidays);

        return Build(month, (DateOnly?)today, holidays, firstDayOfWeek);
    }

    private MonthGrid Build(
        CalendarMonth month,
        DateOnly? today,
        IReadOnlyCollection<HolidayDay>? holidays,
        DayOfWeek firstDayOfWeek)
    {
        if (!Enum.IsDefined(firstDayOfWeek))
        {
            throw new ArgumentOutOfRangeException(
                nameof(firstDayOfWeek),
                firstDayOfWeek,
                "First day of week is not supported.");
        }

        DateOnly firstVisibleDate = GetFirstVisibleDate(month.FirstDay, firstDayOfWeek);
        IReadOnlyDictionary<DateOnly, HolidayDay> holidaysByDate = CreateHolidayLookup(holidays);
        CalendarDay[] days = Enumerable
            .Range(0, MonthGrid.CellCount)
            .Select(offset => CreateDay(firstVisibleDate.AddDays(offset), month, today, holidaysByDate))
            .ToArray();

        return new MonthGrid(month, days);
    }

    private static CalendarDay CreateDay(
        DateOnly date,
        CalendarMonth month,
        DateOnly? today,
        IReadOnlyDictionary<DateOnly, HolidayDay> holidaysByDate)
    {
        DayMarker marker = GetMarker(date, holidaysByDate);

        return new CalendarDay(
            date,
            isInCurrentMonth: date.Year == month.Year && date.Month == month.Month,
            isToday: date == today,
            isWeekend: marker != DayMarker.AdjustedWorkday
                && date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday,
            marker);
    }

    private static DateOnly GetFirstVisibleDate(DateOnly firstOfMonth, DayOfWeek firstDayOfWeek)
    {
        int daysSinceFirstDayOfWeek = ((int)firstOfMonth.DayOfWeek - (int)firstDayOfWeek + 7) % 7;

        return firstOfMonth.AddDays(-daysSinceFirstDayOfWeek);
    }

    private static IReadOnlyDictionary<DateOnly, HolidayDay> CreateHolidayLookup(
        IReadOnlyCollection<HolidayDay>? holidays)
    {
        if (holidays is null)
        {
            return new Dictionary<DateOnly, HolidayDay>();
        }

        Dictionary<DateOnly, HolidayDay> holidaysByDate = new(holidays.Count);

        foreach (HolidayDay holiday in holidays)
        {
            if (!holidaysByDate.TryAdd(holiday.Date, holiday))
            {
                throw new ArgumentException(
                    $"Holiday date '{holiday.Date:yyyy-MM-dd}' appears more than once.",
                    nameof(holidays));
            }
        }

        return holidaysByDate;
    }

    private static DayMarker GetMarker(
        DateOnly date,
        IReadOnlyDictionary<DateOnly, HolidayDay> holidaysByDate)
    {
        if (!holidaysByDate.TryGetValue(date, out HolidayDay? holiday))
        {
            return DayMarker.None;
        }

        return holiday.Type switch
        {
            HolidayDayType.DayOff => DayMarker.DayOff,
            HolidayDayType.AdjustedWorkday => DayMarker.AdjustedWorkday,
            HolidayDayType.FestivalOnly => DayMarker.FestivalOnly,
            _ => throw new ArgumentOutOfRangeException(nameof(holiday), holiday.Type, "Holiday day type is not supported."),
        };
    }
}
