using ChinaTrayCalendar.Domain;
using ChinaTrayCalendar.Infrastructure.Holidays;

namespace ChinaTrayCalendar.Infrastructure.Tests;

public sealed class HolidayJsonParserTests
{
    private readonly HolidayJsonParser parser = new();

    [Fact]
    public void ParseReturnsValidatedHolidayData()
    {
        HolidayDataFile data = parser.Parse(CreateJson(), expectedYear: 2026);

        Assert.Equal(2026, data.Year);
        Assert.Equal("Official notice", data.Source.Title);
        Assert.Equal(new DateOnly(2025, 11, 4), data.Source.PublishedDate);
        Assert.Equal("https://example.invalid/official-notice", data.Source.Url);
        Assert.Collection(
            data.Days,
            day =>
            {
                Assert.Equal(new DateOnly(2026, 1, 1), day.Date);
                Assert.Equal(HolidayDayType.DayOff, day.Type);
                Assert.Equal("Yuan Dan", day.Name);
            },
            day =>
            {
                Assert.Equal(new DateOnly(2026, 1, 4), day.Date);
                Assert.Equal(HolidayDayType.AdjustedWorkday, day.Type);
                Assert.Equal("Adjusted workday", day.Name);
            },
            day =>
            {
                Assert.Equal(new DateOnly(2026, 2, 14), day.Date);
                Assert.Equal(HolidayDayType.FestivalOnly, day.Type);
                Assert.Equal("Festival only", day.Name);
            });
    }

    [Fact]
    public void ParseRejectsInvalidDate()
    {
        string json = CreateJson(dayDate: "2026-02-30");

        HolidayDataException exception = Assert.Throws<HolidayDataException>(() => parser.Parse(json, 2026));

        Assert.Contains("Holiday day date '2026-02-30' must be an ISO date string.", exception.Message);
    }

    [Fact]
    public void ParseRejectsDuplicateDate()
    {
        string json = CreateJson(extraDay: """
            { "date": "2026-01-01", "type": "dayOff", "name": "Duplicate" }
            """);

        HolidayDataException exception = Assert.Throws<HolidayDataException>(() => parser.Parse(json, 2026));

        Assert.Contains("Duplicate holiday day date '2026-01-01'.", exception.Message);
    }

    [Fact]
    public void ParseRejectsUnknownType()
    {
        string json = CreateJson(dayType: "unknown");

        HolidayDataException exception = Assert.Throws<HolidayDataException>(() => parser.Parse(json, 2026));

        Assert.Contains("Unknown holiday day type 'unknown'.", exception.Message);
    }

    [Fact]
    public void ParseRejectsMismatchedYear()
    {
        string json = CreateJson(year: 2025);

        HolidayDataException exception = Assert.Throws<HolidayDataException>(() => parser.Parse(json, 2026));

        Assert.Contains("Holiday data year '2025' does not match expected year '2026'.", exception.Message);
    }

    [Fact]
    public void ParseRejectsDayDateOutsideYear()
    {
        string json = CreateJson(dayDate: "2025-12-31");

        HolidayDataException exception = Assert.Throws<HolidayDataException>(() => parser.Parse(json, 2026));

        Assert.Contains("Holiday day '2025-12-31' does not belong to year '2026'.", exception.Message);
    }

    [Fact]
    public void ParseRejectsUnsupportedJurisdiction()
    {
        string json = CreateJson(jurisdiction: "US");

        HolidayDataException exception = Assert.Throws<HolidayDataException>(() => parser.Parse(json, 2026));

        Assert.Contains("Unsupported holiday jurisdiction 'US'.", exception.Message);
    }

    [Fact]
    public void ParseRejectsUnsupportedSchemaVersion()
    {
        string json = CreateJson(schemaVersion: 2);

        HolidayDataException exception = Assert.Throws<HolidayDataException>(() => parser.Parse(json, 2026));

        Assert.Contains("Unsupported holiday schemaVersion '2'.", exception.Message);
    }

    private static string CreateJson(
        int schemaVersion = 1,
        string jurisdiction = "CN",
        int year = 2026,
        string dayDate = "2026-01-01",
        string dayType = "dayOff",
        string? extraDay = null)
    {
        string extraDaySegment = string.IsNullOrWhiteSpace(extraDay)
            ? string.Empty
            : $",{Environment.NewLine}{extraDay}";

        return $$"""
            {
              "schemaVersion": {{schemaVersion}},
              "jurisdiction": "{{jurisdiction}}",
              "year": {{year}},
              "source": {
                "title": "Official notice",
                "publishedDate": "2025-11-04",
                "url": "https://example.invalid/official-notice"
              },
              "days": [
                { "date": "{{dayDate}}", "type": "{{dayType}}", "name": "Yuan Dan" },
                { "date": "2026-01-04", "type": "adjustedWorkday", "name": "Adjusted workday" },
                { "date": "2026-02-14", "type": "festivalOnly", "name": "Festival only" }{{extraDaySegment}}
              ]
            }
            """;
    }
}
