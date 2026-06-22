using ChinaTrayCalendar.Application.Settings;

namespace ChinaTrayCalendar.Application.Tests;

public sealed class AppSettingsTests
{
    [Fact]
    public void CreateDefaultUsesMvpDefaults()
    {
        AppSettings settings = AppSettings.CreateDefault();

        Assert.Equal(DayOfWeek.Monday, settings.FirstDayOfWeek);
        Assert.False(settings.StartWithWindows);
        Assert.Equal(AppTheme.System, settings.Theme);
    }

    [Fact]
    public void ConstructorStoresValues()
    {
        AppSettings settings = new(DayOfWeek.Sunday, startWithWindows: true, AppTheme.Dark);

        Assert.Equal(DayOfWeek.Sunday, settings.FirstDayOfWeek);
        Assert.True(settings.StartWithWindows);
        Assert.Equal(AppTheme.Dark, settings.Theme);
    }

    [Fact]
    public void ConstructorRejectsUnsupportedFirstDayOfWeek()
    {
        const DayOfWeek unsupportedDayOfWeek = (DayOfWeek)99;

        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new AppSettings(unsupportedDayOfWeek, startWithWindows: false));

        Assert.Equal("firstDayOfWeek", exception.ParamName);
    }

    [Fact]
    public void ConstructorRejectsUnsupportedTheme()
    {
        const AppTheme unsupportedTheme = (AppTheme)99;

        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new AppSettings(DayOfWeek.Monday, startWithWindows: false, unsupportedTheme));

        Assert.Equal("theme", exception.ParamName);
    }
}
