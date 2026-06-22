using ChinaTrayCalendar.Domain;

namespace ChinaTrayCalendar.Domain.Tests;

public sealed class CalendarDayTests
{
    [Fact]
    public void ConstructorStoresClassificationValues()
    {
        DateOnly date = new(2026, 10, 1);

        CalendarDay day = new(
            date,
            isInCurrentMonth: true,
            isToday: true,
            isWeekend: false,
            marker: DayMarker.DayOff);

        Assert.Equal(date, day.Date);
        Assert.True(day.IsInCurrentMonth);
        Assert.True(day.IsToday);
        Assert.False(day.IsWeekend);
        Assert.Equal(DayMarker.DayOff, day.Marker);
    }

    [Fact]
    public void ConstructorRejectsUnsupportedMarker()
    {
        const DayMarker unsupportedMarker = (DayMarker)99;

        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new CalendarDay(new DateOnly(2026, 1, 1), isInCurrentMonth: true, marker: unsupportedMarker));

        Assert.Equal("marker", exception.ParamName);
    }
}
