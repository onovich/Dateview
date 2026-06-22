using ChinaTrayCalendar.Domain;

namespace ChinaTrayCalendar.Domain.Tests;

public sealed class HolidayDayTests
{
    [Fact]
    public void ConstructorStoresHolidayValues()
    {
        DateOnly date = new(2026, 1, 1);

        HolidayDay day = new(date, HolidayDayType.DayOff, "Yuan Dan");

        Assert.Equal(date, day.Date);
        Assert.Equal(HolidayDayType.DayOff, day.Type);
        Assert.Equal("Yuan Dan", day.Name);
    }

    [Fact]
    public void ConstructorRejectsUnsupportedType()
    {
        const HolidayDayType unsupportedType = (HolidayDayType)99;

        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new HolidayDay(new DateOnly(2026, 1, 1), unsupportedType, "Name"));

        Assert.Equal("type", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ConstructorRejectsBlankName(string name)
    {
        Assert.Throws<ArgumentException>(() => new HolidayDay(new DateOnly(2026, 1, 1), HolidayDayType.DayOff, name));
    }
}
