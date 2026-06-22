using ChinaTrayCalendar.Domain;

namespace ChinaTrayCalendar.Domain.Tests;

public sealed class MonthGridBuilderTests
{
    private readonly MonthGridBuilder builder = new();

    [Fact]
    public void BuildUsesMondayAsDefaultFirstDayWhenMonthStartsOnMonday()
    {
        MonthGrid grid = builder.Build(new CalendarMonth(2026, 6));

        AssertGridDates(grid, new DateOnly(2026, 6, 1), new DateOnly(2026, 7, 12));
        Assert.Equal(30, grid.Days.Count(day => day.IsInCurrentMonth));
        Assert.All(grid.Days.Take(30), day => Assert.True(day.IsInCurrentMonth));
        Assert.All(grid.Days.Skip(30), day => Assert.False(day.IsInCurrentMonth));
    }

    [Fact]
    public void BuildIncludesPreviousMonthFillersWhenMonthStartsOnSunday()
    {
        MonthGrid grid = builder.Build(new CalendarMonth(2026, 2));

        AssertGridDates(grid, new DateOnly(2026, 1, 26), new DateOnly(2026, 3, 8));
        Assert.Equal(28, grid.Days.Count(day => day.IsInCurrentMonth));
        Assert.All(grid.Days.Take(6), day => Assert.False(day.IsInCurrentMonth));
    }

    [Fact]
    public void BuildIncludesBothSideFillersWhenMonthStartsMidweek()
    {
        MonthGrid grid = builder.Build(new CalendarMonth(2026, 4));

        AssertGridDates(grid, new DateOnly(2026, 3, 30), new DateOnly(2026, 5, 10));
        Assert.Equal(30, grid.Days.Count(day => day.IsInCurrentMonth));
        Assert.False(grid.Days[1].IsInCurrentMonth);
        Assert.True(grid.Days[2].IsInCurrentMonth);
        Assert.False(grid.Days[32].IsInCurrentMonth);
    }

    [Fact]
    public void BuildSupportsCustomFirstDayOfWeek()
    {
        MonthGrid grid = builder.Build(new CalendarMonth(2026, 6), DayOfWeek.Sunday);

        AssertGridDates(grid, new DateOnly(2026, 5, 31), new DateOnly(2026, 7, 11));
    }

    [Fact]
    public void BuildHandlesLeapYearFebruary()
    {
        MonthGrid grid = builder.Build(new CalendarMonth(2024, 2));

        AssertGridDates(grid, new DateOnly(2024, 1, 29), new DateOnly(2024, 3, 10));
        Assert.Equal(29, grid.Days.Count(day => day.IsInCurrentMonth));
        Assert.Contains(grid.Days, day => day.Date == new DateOnly(2024, 2, 29) && day.IsInCurrentMonth);
    }

    [Fact]
    public void BuildRejectsUnsupportedFirstDayOfWeek()
    {
        const DayOfWeek unsupportedDayOfWeek = (DayOfWeek)99;

        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => builder.Build(new CalendarMonth(2026, 6), unsupportedDayOfWeek));

        Assert.Equal("firstDayOfWeek", exception.ParamName);
    }

    private static void AssertGridDates(MonthGrid grid, DateOnly expectedFirstDate, DateOnly expectedLastDate)
    {
        Assert.Equal(MonthGrid.CellCount, grid.Days.Count);
        Assert.Equal(expectedFirstDate, grid.Days[0].Date);
        Assert.Equal(expectedLastDate, grid.Days[^1].Date);
    }
}
