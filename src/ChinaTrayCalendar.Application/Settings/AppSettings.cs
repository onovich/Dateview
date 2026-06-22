namespace ChinaTrayCalendar.Application.Settings;

public sealed record AppSettings
{
    public AppSettings(
        DayOfWeek firstDayOfWeek,
        bool startWithWindows,
        AppTheme theme = AppTheme.System)
    {
        if (!Enum.IsDefined(firstDayOfWeek))
        {
            throw new ArgumentOutOfRangeException(
                nameof(firstDayOfWeek),
                firstDayOfWeek,
                "First day of week is not supported.");
        }

        if (!Enum.IsDefined(theme))
        {
            throw new ArgumentOutOfRangeException(nameof(theme), theme, "App theme is not supported.");
        }

        FirstDayOfWeek = firstDayOfWeek;
        StartWithWindows = startWithWindows;
        Theme = theme;
    }

    public DayOfWeek FirstDayOfWeek { get; }

    public bool StartWithWindows { get; }

    public AppTheme Theme { get; }

    public static AppSettings CreateDefault()
    {
        return new AppSettings(DayOfWeek.Monday, startWithWindows: false);
    }
}
