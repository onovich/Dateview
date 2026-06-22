using ChinaTrayCalendar.Domain;

namespace ChinaTrayCalendar.Application.Calendars;

public sealed class MonthCalendarDto
{
    public MonthCalendarDto(
        CalendarMonth month,
        IReadOnlyList<CalendarDayDto> days,
        IReadOnlyList<int> missingHolidayYears)
    {
        ArgumentNullException.ThrowIfNull(days);
        ArgumentNullException.ThrowIfNull(missingHolidayYears);

        if (days.Count != MonthGrid.CellCount)
        {
            throw new ArgumentException(
                $"Month calendar must contain exactly {MonthGrid.CellCount} days.",
                nameof(days));
        }

        Month = month;
        Days = days.ToArray();
        MissingHolidayYears = missingHolidayYears.Distinct().Order().ToArray();
    }

    public CalendarMonth Month { get; }

    public IReadOnlyList<CalendarDayDto> Days { get; }

    public IReadOnlyList<int> MissingHolidayYears { get; }

    public bool HasMissingHolidayData => MissingHolidayYears.Count > 0;
}
