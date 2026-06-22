using ChinaTrayCalendar.Domain;

namespace ChinaTrayCalendar.Domain.Tests;

public sealed class MonthGridTests
{
    [Fact]
    public void ConstructorAcceptsExactlyFortyTwoDays()
    {
        CalendarMonth month = new(2026, 6);
        CalendarDay[] days = Enumerable
            .Range(0, MonthGrid.CellCount)
            .Select(offset => new CalendarDay(month.FirstDay.AddDays(offset), isInCurrentMonth: true))
            .ToArray();

        MonthGrid grid = new(month, days);

        Assert.Equal(month, grid.Month);
        Assert.Equal(MonthGrid.CellCount, grid.Days.Count);
    }

    [Fact]
    public void ConstructorRejectsNullDays()
    {
        Assert.Throws<ArgumentNullException>(() => new MonthGrid(new CalendarMonth(2026, 6), days: null!));
    }

    [Fact]
    public void ConstructorRejectsUnexpectedDayCount()
    {
        CalendarDay[] days =
        [
            new(new DateOnly(2026, 6, 1), isInCurrentMonth: true),
        ];

        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => new MonthGrid(new CalendarMonth(2026, 6), days));

        Assert.Equal("days", exception.ParamName);
    }
}
