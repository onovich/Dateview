using ChinaTrayCalendar.Application.Calendars;
using ChinaTrayCalendar.Domain;

namespace ChinaTrayCalendar.Application.Tests;

public sealed class CalendarDtoTests
{
    [Fact]
    public void CalendarDayDtoStoresValues()
    {
        DateOnly date = new(2026, 1, 1);

        CalendarDayDto day = new(
            date,
            isInCurrentMonth: true,
            isToday: false,
            isWeekend: false,
            DayMarker.DayOff,
            "Yuan Dan");

        Assert.Equal(date, day.Date);
        Assert.True(day.IsInCurrentMonth);
        Assert.False(day.IsToday);
        Assert.False(day.IsWeekend);
        Assert.Equal(DayMarker.DayOff, day.Marker);
        Assert.Equal("Yuan Dan", day.HolidayName);
    }

    [Fact]
    public void CalendarDayDtoRejectsUnsupportedMarker()
    {
        const DayMarker unsupportedMarker = (DayMarker)99;

        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new CalendarDayDto(
                new DateOnly(2026, 1, 1),
                isInCurrentMonth: true,
                isToday: false,
                isWeekend: false,
                unsupportedMarker));

        Assert.Equal("marker", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CalendarDayDtoRejectsBlankHolidayNameWhenProvided(string holidayName)
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => new CalendarDayDto(
                new DateOnly(2026, 1, 1),
                isInCurrentMonth: true,
                isToday: false,
                isWeekend: false,
                DayMarker.DayOff,
                holidayName));

        Assert.Equal("holidayName", exception.ParamName);
    }

    [Fact]
    public void MonthCalendarDtoStoresValues()
    {
        CalendarMonth month = new(2026, 1);
        CalendarDayDto[] days = CreateDays(month);

        MonthCalendarDto calendar = new(month, days, [2027, 2027, 2025]);

        Assert.Equal(month, calendar.Month);
        Assert.Equal(MonthGrid.CellCount, calendar.Days.Count);
        Assert.Equal([2025, 2027], calendar.MissingHolidayYears);
        Assert.True(calendar.HasMissingHolidayData);
    }

    [Fact]
    public void MonthCalendarDtoRejectsUnexpectedDayCount()
    {
        CalendarDayDto[] days =
        [
            new(
                new DateOnly(2026, 1, 1),
                isInCurrentMonth: true,
                isToday: false,
                isWeekend: false,
                DayMarker.None),
        ];

        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => new MonthCalendarDto(new CalendarMonth(2026, 1), days, []));

        Assert.Equal("days", exception.ParamName);
    }

    private static CalendarDayDto[] CreateDays(CalendarMonth month)
    {
        return Enumerable
            .Range(0, MonthGrid.CellCount)
            .Select(offset => new CalendarDayDto(
                month.FirstDay.AddDays(offset),
                isInCurrentMonth: true,
                isToday: false,
                isWeekend: false,
                DayMarker.None))
            .ToArray();
    }
}
