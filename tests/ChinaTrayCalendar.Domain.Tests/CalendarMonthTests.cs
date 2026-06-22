using ChinaTrayCalendar.Domain;

namespace ChinaTrayCalendar.Domain.Tests;

public sealed class CalendarMonthTests
{
    [Theory]
    [InlineData(1900, 1)]
    [InlineData(2026, 6)]
    [InlineData(2100, 12)]
    public void ConstructorAcceptsSupportedMonth(int year, int month)
    {
        CalendarMonth calendarMonth = new(year, month);

        Assert.Equal(year, calendarMonth.Year);
        Assert.Equal(month, calendarMonth.Month);
        Assert.Equal(new DateOnly(year, month, 1), calendarMonth.FirstDay);
        Assert.Equal($"{year:D4}-{month:D2}", calendarMonth.ToString());
    }

    [Theory]
    [InlineData(1899)]
    [InlineData(2101)]
    public void ConstructorRejectsUnsupportedYear(int year)
    {
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new CalendarMonth(year, 1));

        Assert.Equal("year", exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void ConstructorRejectsUnsupportedMonth(int month)
    {
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new CalendarMonth(2026, month));

        Assert.Equal("month", exception.ParamName);
    }

    [Fact]
    public void AddMonthsReturnsValidatedTargetMonth()
    {
        CalendarMonth month = new(2026, 1);

        CalendarMonth next = month.AddMonths(1);

        Assert.Equal(new CalendarMonth(2026, 2), next);
    }
}
