using System.Text;
using ChinaTrayCalendar.Application.Ports;
using ChinaTrayCalendar.Domain;
using ChinaTrayCalendar.Infrastructure.Holidays;

namespace ChinaTrayCalendar.Infrastructure.Tests;

public sealed class JsonHolidayRepositoryTests
{
    [Fact]
    public void ConstructorRejectsBlankDirectoryPath()
    {
        Assert.Throws<ArgumentException>(() => new JsonHolidayRepository("   "));
    }

    [Fact]
    public async Task GetHolidayDaysAsyncReturnsParsedDays()
    {
        using TestHolidayDirectory directory = new();
        directory.WriteYear(2026, CreateJson("Yuan Dan"));
        JsonHolidayRepository repository = new(directory.DirectoryPath);

        IReadOnlyList<HolidayDay> days = await repository.GetHolidayDaysAsync(2026, CancellationToken.None);

        HolidayDay day = Assert.Single(days);
        Assert.Equal(new DateOnly(2026, 1, 1), day.Date);
        Assert.Equal(HolidayDayType.DayOff, day.Type);
        Assert.Equal("Yuan Dan", day.Name);
    }

    [Fact]
    public async Task GetHolidayDaysAsyncCachesParsedYear()
    {
        using TestHolidayDirectory directory = new();
        directory.WriteYear(2026, CreateJson("First read"));
        JsonHolidayRepository repository = new(directory.DirectoryPath);

        IReadOnlyList<HolidayDay> first = await repository.GetHolidayDaysAsync(2026, CancellationToken.None);
        directory.WriteYear(2026, CreateJson("Second read"));
        IReadOnlyList<HolidayDay> second = await repository.GetHolidayDaysAsync(2026, CancellationToken.None);

        Assert.Same(first, second);
        Assert.Equal("First read", Assert.Single(second).Name);
    }

    [Fact]
    public async Task GetHolidayDaysAsyncRejectsMissingFile()
    {
        using TestHolidayDirectory directory = new();
        JsonHolidayRepository repository = new(directory.DirectoryPath);

        HolidayDataUnavailableException exception = await Assert.ThrowsAsync<HolidayDataUnavailableException>(
            () => repository.GetHolidayDaysAsync(2026, CancellationToken.None));

        Assert.Equal(2026, exception.Year);
        Assert.Contains("Holiday data file for year '2026' was not found", exception.Message);
    }

    [Fact]
    public async Task GetHolidayDaysAsyncCanBeUsedThroughApplicationPort()
    {
        using TestHolidayDirectory directory = new();
        directory.WriteYear(2026, CreateJson("Yuan Dan"));
        IHolidayRepository repository = new JsonHolidayRepository(directory.DirectoryPath);

        IReadOnlyList<HolidayDay> days = await repository.GetHolidayDaysAsync(2026, CancellationToken.None);

        Assert.Equal("Yuan Dan", Assert.Single(days).Name);
    }

    [Fact]
    public async Task GetHolidayDaysAsyncRejectsInvalidFile()
    {
        using TestHolidayDirectory directory = new();
        directory.WriteYear(2026, CreateJson("Invalid", type: "unknown"));
        JsonHolidayRepository repository = new(directory.DirectoryPath);

        HolidayDataException exception = await Assert.ThrowsAsync<HolidayDataException>(
            () => repository.GetHolidayDaysAsync(2026, CancellationToken.None));

        Assert.Contains("Unknown holiday day type 'unknown'.", exception.Message);
    }

    private static string CreateJson(string name, string type = "dayOff")
    {
        return $$"""
            {
              "schemaVersion": 1,
              "jurisdiction": "CN",
              "year": 2026,
              "source": {
                "title": "Official notice",
                "publishedDate": "2025-11-04"
              },
              "days": [
                { "date": "2026-01-01", "type": "{{type}}", "name": "{{name}}" }
              ]
            }
            """;
    }

    private sealed class TestHolidayDirectory : IDisposable
    {
        public TestHolidayDirectory()
        {
            DirectoryPath = Path.Combine(
                Path.GetTempPath(),
                $"Dateview-HolidayRepositoryTests-{Guid.NewGuid():N}");
            Directory.CreateDirectory(DirectoryPath);
        }

        public string DirectoryPath { get; }

        public void WriteYear(int year, string json)
        {
            File.WriteAllText(Path.Combine(DirectoryPath, $"{year:D4}.json"), json, Encoding.UTF8);
        }

        public void Dispose()
        {
            if (Directory.Exists(DirectoryPath))
            {
                Directory.Delete(DirectoryPath, recursive: true);
            }
        }
    }
}
