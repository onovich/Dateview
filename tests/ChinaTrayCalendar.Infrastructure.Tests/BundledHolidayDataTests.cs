using ChinaTrayCalendar.Domain;
using ChinaTrayCalendar.Infrastructure.Holidays;

namespace ChinaTrayCalendar.Infrastructure.Tests;

public sealed class BundledHolidayDataTests
{
    [Theory]
    [InlineData(2025, 28, 5)]
    [InlineData(2026, 33, 6)]
    public async Task BundledHolidayDataParsesAndHasExpectedCounts(
        int year,
        int expectedDayOffCount,
        int expectedAdjustedWorkdayCount)
    {
        JsonHolidayRepository repository = new(GetHolidayDirectoryPath());

        IReadOnlyList<HolidayDay> days = await repository.GetHolidayDaysAsync(year, CancellationToken.None);

        Assert.Equal(expectedDayOffCount, days.Count(day => day.Type == HolidayDayType.DayOff));
        Assert.Equal(
            expectedAdjustedWorkdayCount,
            days.Count(day => day.Type == HolidayDayType.AdjustedWorkday));
    }

    [Fact]
    public async Task Bundled2025HolidayDataContainsOfficialAdjustedWorkdays()
    {
        JsonHolidayRepository repository = new(GetHolidayDirectoryPath());

        IReadOnlyList<HolidayDay> days = await repository.GetHolidayDaysAsync(2025, CancellationToken.None);

        AssertAdjustedWorkdays(
            days,
            new DateOnly(2025, 1, 26),
            new DateOnly(2025, 2, 8),
            new DateOnly(2025, 4, 27),
            new DateOnly(2025, 9, 28),
            new DateOnly(2025, 10, 11));
    }

    [Fact]
    public async Task Bundled2026HolidayDataContainsOfficialAdjustedWorkdays()
    {
        JsonHolidayRepository repository = new(GetHolidayDirectoryPath());

        IReadOnlyList<HolidayDay> days = await repository.GetHolidayDaysAsync(2026, CancellationToken.None);

        AssertAdjustedWorkdays(
            days,
            new DateOnly(2026, 1, 4),
            new DateOnly(2026, 2, 14),
            new DateOnly(2026, 2, 28),
            new DateOnly(2026, 5, 9),
            new DateOnly(2026, 9, 20),
            new DateOnly(2026, 10, 10));
    }

    private static void AssertAdjustedWorkdays(IReadOnlyList<HolidayDay> days, params DateOnly[] expectedDates)
    {
        HashSet<DateOnly> actualDates = days
            .Where(day => day.Type == HolidayDayType.AdjustedWorkday)
            .Select(day => day.Date)
            .ToHashSet();

        Assert.Equal(expectedDates.Length, actualDates.Count);
        foreach (DateOnly expectedDate in expectedDates)
        {
            Assert.Contains(expectedDate, actualDates);
        }
    }

    private static string GetHolidayDirectoryPath()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Dateview.slnx")))
        {
            directory = directory.Parent;
        }

        if (directory is null)
        {
            throw new InvalidOperationException("Repository root could not be found.");
        }

        return Path.Combine(directory.FullName, "assets", "holidays", "cn");
    }
}
