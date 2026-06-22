using ChinaTrayCalendar.Application.Ports;
using ChinaTrayCalendar.Domain;

namespace ChinaTrayCalendar.Application.Calendars;

public sealed class GetMonthCalendarUseCase
{
    private readonly IClock clock;
    private readonly MonthGridBuilder gridBuilder;
    private readonly IHolidayRepository holidayRepository;

    public GetMonthCalendarUseCase(IClock clock, IHolidayRepository holidayRepository)
        : this(clock, holidayRepository, new MonthGridBuilder())
    {
    }

    public GetMonthCalendarUseCase(
        IClock clock,
        IHolidayRepository holidayRepository,
        MonthGridBuilder gridBuilder)
    {
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(holidayRepository);
        ArgumentNullException.ThrowIfNull(gridBuilder);

        this.clock = clock;
        this.holidayRepository = holidayRepository;
        this.gridBuilder = gridBuilder;
    }

    public async Task<MonthCalendarDto> ExecuteAsync(
        CalendarMonth month,
        DayOfWeek firstDayOfWeek,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        DateOnly today = clock.Today;
        MonthGrid visibleGrid = gridBuilder.Build(month, today, firstDayOfWeek);
        int[] visibleYears = visibleGrid.Days
            .Select(day => day.Date.Year)
            .Distinct()
            .Order()
            .ToArray();

        List<HolidayDay> holidays = [];
        List<int> missingHolidayYears = [];

        foreach (int year in visibleYears)
        {
            try
            {
                IReadOnlyList<HolidayDay> yearHolidays =
                    await holidayRepository.GetHolidayDaysAsync(year, cancellationToken).ConfigureAwait(false);
                holidays.AddRange(yearHolidays);
            }
            catch (HolidayDataUnavailableException exception) when (exception.Year == year)
            {
                missingHolidayYears.Add(year);
            }
        }

        MonthGrid classifiedGrid = gridBuilder.Build(month, today, holidays, firstDayOfWeek);
        IReadOnlyDictionary<DateOnly, HolidayDay> holidaysByDate = holidays.ToDictionary(day => day.Date);
        CalendarDayDto[] days = classifiedGrid.Days
            .Select(day => ToDto(day, holidaysByDate))
            .ToArray();

        return new MonthCalendarDto(month, days, missingHolidayYears);
    }

    private static CalendarDayDto ToDto(
        CalendarDay day,
        IReadOnlyDictionary<DateOnly, HolidayDay> holidaysByDate)
    {
        string? holidayName = holidaysByDate.TryGetValue(day.Date, out HolidayDay? holiday)
            ? holiday.Name
            : null;

        return new CalendarDayDto(
            day.Date,
            day.IsInCurrentMonth,
            day.IsToday,
            day.IsWeekend,
            day.Marker,
            holidayName);
    }
}
