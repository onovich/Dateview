using ChinaTrayCalendar.Application.Calendars;
using ChinaTrayCalendar.Application.Ports;
using ChinaTrayCalendar.Domain;

namespace ChinaTrayCalendar.Application.Tests;

public sealed class GetMonthCalendarUseCaseTests
{
    [Fact]
    public async Task ExecuteAsyncUsesClockForToday()
    {
        FakeHolidayRepository holidays = new();
        holidays.SetYear(2026, []);
        GetMonthCalendarUseCase useCase = new(new FakeClock(new DateOnly(2026, 6, 22)), holidays);

        MonthCalendarDto calendar = await useCase.ExecuteAsync(
            new CalendarMonth(2026, 6),
            DayOfWeek.Monday,
            CancellationToken.None);

        CalendarDayDto today = Assert.Single(calendar.Days, day => day.IsToday);
        Assert.Equal(new DateOnly(2026, 6, 22), today.Date);
    }

    [Fact]
    public async Task ExecuteAsyncLoadsHolidayDataForVisibleYears()
    {
        FakeHolidayRepository holidays = new();
        holidays.SetYear(2025, [new HolidayDay(new DateOnly(2025, 12, 31), HolidayDayType.DayOff, "Previous year")]);
        holidays.SetYear(2026, [new HolidayDay(new DateOnly(2026, 1, 1), HolidayDayType.DayOff, "Current year")]);
        GetMonthCalendarUseCase useCase = new(new FakeClock(new DateOnly(2026, 1, 1)), holidays);

        MonthCalendarDto calendar = await useCase.ExecuteAsync(
            new CalendarMonth(2026, 1),
            DayOfWeek.Monday,
            CancellationToken.None);

        Assert.Equal([2025, 2026], holidays.RequestedYears);
        AssertHoliday(calendar, new DateOnly(2025, 12, 31), "Previous year");
        AssertHoliday(calendar, new DateOnly(2026, 1, 1), "Current year");
        Assert.False(calendar.HasMissingHolidayData);
    }

    [Fact]
    public async Task ExecuteAsyncRecordsMissingHolidayYearsWithoutInventingMarkers()
    {
        FakeHolidayRepository holidays = new();
        holidays.MarkMissing(2025);
        holidays.SetYear(2026, [new HolidayDay(new DateOnly(2026, 1, 1), HolidayDayType.DayOff, "Current year")]);
        GetMonthCalendarUseCase useCase = new(new FakeClock(new DateOnly(2026, 1, 1)), holidays);

        MonthCalendarDto calendar = await useCase.ExecuteAsync(
            new CalendarMonth(2026, 1),
            DayOfWeek.Monday,
            CancellationToken.None);

        Assert.Equal([2025], calendar.MissingHolidayYears);
        Assert.Equal(DayMarker.None, GetDay(calendar, new DateOnly(2025, 12, 31)).Marker);
        AssertHoliday(calendar, new DateOnly(2026, 1, 1), "Current year");
    }

    private static void AssertHoliday(MonthCalendarDto calendar, DateOnly date, string expectedName)
    {
        CalendarDayDto day = GetDay(calendar, date);
        Assert.Equal(DayMarker.DayOff, day.Marker);
        Assert.Equal(expectedName, day.HolidayName);
    }

    private static CalendarDayDto GetDay(MonthCalendarDto calendar, DateOnly date)
    {
        return Assert.Single(calendar.Days, day => day.Date == date);
    }

    private sealed class FakeClock(DateOnly today) : IClock
    {
        public DateOnly Today { get; } = today;
    }

    private sealed class FakeHolidayRepository : IHolidayRepository
    {
        private readonly Dictionary<int, IReadOnlyList<HolidayDay>> holidaysByYear = [];
        private readonly HashSet<int> missingYears = [];

        public List<int> RequestedYears { get; } = [];

        public Task<IReadOnlyList<HolidayDay>> GetHolidayDaysAsync(
            int year,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            RequestedYears.Add(year);

            if (missingYears.Contains(year))
            {
                throw new HolidayDataUnavailableException(year, $"Missing {year} holiday data.");
            }

            return Task.FromResult(holidaysByYear.TryGetValue(year, out IReadOnlyList<HolidayDay>? holidays)
                ? holidays
                : Array.Empty<HolidayDay>());
        }

        public void SetYear(int year, IReadOnlyList<HolidayDay> holidays)
        {
            holidaysByYear[year] = holidays;
        }

        public void MarkMissing(int year)
        {
            missingYears.Add(year);
        }
    }
}
