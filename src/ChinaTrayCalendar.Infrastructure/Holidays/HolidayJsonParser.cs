using System.Globalization;
using System.Text.Json;
using ChinaTrayCalendar.Domain;

namespace ChinaTrayCalendar.Infrastructure.Holidays;

public sealed class HolidayJsonParser
{
    private const int SupportedSchemaVersion = 1;
    private const string SupportedJurisdiction = "CN";
    private const string DateFormat = "yyyy-MM-dd";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = false,
    };

    public HolidayDataFile Parse(string json, int expectedYear)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        ValidateYear(expectedYear);

        HolidayFileDto dto = Deserialize(json);
        ValidateHeader(dto, expectedYear);

        HolidaySource source = CreateSource(dto.Source);
        IReadOnlyList<HolidayDay> days = CreateDays(dto.Days, expectedYear);

        return new HolidayDataFile(dto.Year, source, days);
    }

    private static HolidayFileDto Deserialize(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<HolidayFileDto>(json, SerializerOptions)
                ?? throw new HolidayDataException("Holiday data JSON must contain an object.");
        }
        catch (JsonException exception)
        {
            throw new HolidayDataException("Holiday data JSON is invalid.", exception);
        }
    }

    private static void ValidateHeader(HolidayFileDto dto, int expectedYear)
    {
        if (dto.SchemaVersion != SupportedSchemaVersion)
        {
            throw new HolidayDataException($"Unsupported holiday schemaVersion '{dto.SchemaVersion}'.");
        }

        if (!string.Equals(dto.Jurisdiction, SupportedJurisdiction, StringComparison.Ordinal))
        {
            throw new HolidayDataException($"Unsupported holiday jurisdiction '{dto.Jurisdiction}'.");
        }

        if (dto.Year != expectedYear)
        {
            throw new HolidayDataException(
                $"Holiday data year '{dto.Year}' does not match expected year '{expectedYear}'.");
        }
    }

    private static HolidaySource CreateSource(HolidaySourceDto? source)
    {
        if (source is null)
        {
            throw new HolidayDataException("Holiday source is required.");
        }

        string title = RequireText(source.Title, "Holiday source title");
        DateOnly publishedDate = ParseDate(source.PublishedDate, "Holiday source publishedDate");

        try
        {
            return new HolidaySource(title, publishedDate, source.Url);
        }
        catch (ArgumentException exception)
        {
            throw new HolidayDataException("Holiday source metadata is invalid.", exception);
        }
    }

    private static IReadOnlyList<HolidayDay> CreateDays(IReadOnlyList<HolidayDayDto>? days, int expectedYear)
    {
        if (days is null)
        {
            throw new HolidayDataException("Holiday days are required.");
        }

        HashSet<DateOnly> seenDates = [];
        List<HolidayDay> result = new(days.Count);

        foreach (HolidayDayDto day in days)
        {
            DateOnly date = ParseDate(day.Date, "Holiday day date");
            if (date.Year != expectedYear)
            {
                throw new HolidayDataException(
                    $"Holiday day '{date:yyyy-MM-dd}' does not belong to year '{expectedYear}'.");
            }

            if (!seenDates.Add(date))
            {
                throw new HolidayDataException($"Duplicate holiday day date '{date:yyyy-MM-dd}'.");
            }

            HolidayDayType type = ParseType(day.Type);
            string name = RequireText(day.Name, $"Holiday day '{date:yyyy-MM-dd}' name");

            try
            {
                result.Add(new HolidayDay(date, type, name));
            }
            catch (ArgumentException exception)
            {
                throw new HolidayDataException($"Holiday day '{date:yyyy-MM-dd}' is invalid.", exception);
            }
        }

        return result;
    }

    private static DateOnly ParseDate(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new HolidayDataException($"{fieldName} is required.");
        }

        if (DateOnly.TryParseExact(
            value,
            DateFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateOnly date))
        {
            return date;
        }

        throw new HolidayDataException($"{fieldName} '{value}' must be an ISO date string.");
    }

    private static HolidayDayType ParseType(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new HolidayDataException("Holiday day type is required.");
        }

        return value switch
        {
            "dayOff" => HolidayDayType.DayOff,
            "adjustedWorkday" => HolidayDayType.AdjustedWorkday,
            "festivalOnly" => HolidayDayType.FestivalOnly,
            _ => throw new HolidayDataException($"Unknown holiday day type '{value}'."),
        };
    }

    private static string RequireText(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new HolidayDataException($"{fieldName} is required.");
        }

        return value;
    }

    private static void ValidateYear(int year)
    {
        _ = new CalendarMonth(year, 1);
    }

    private sealed class HolidayFileDto
    {
        public int SchemaVersion { get; init; }

        public string? Jurisdiction { get; init; }

        public int Year { get; init; }

        public HolidaySourceDto? Source { get; init; }

        public IReadOnlyList<HolidayDayDto>? Days { get; init; }
    }

    private sealed class HolidaySourceDto
    {
        public string? Title { get; init; }

        public string? PublishedDate { get; init; }

        public string? Url { get; init; }
    }

    private sealed class HolidayDayDto
    {
        public string? Date { get; init; }

        public string? Type { get; init; }

        public string? Name { get; init; }
    }
}
