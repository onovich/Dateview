using System.Text;
using ChinaTrayCalendar.Domain;

namespace ChinaTrayCalendar.Infrastructure.Holidays;

public sealed class JsonHolidayRepository
{
    private readonly Dictionary<int, IReadOnlyList<HolidayDay>> cache = [];
    private readonly object cacheLock = new();
    private readonly string holidayDirectoryPath;
    private readonly HolidayJsonParser parser;

    public JsonHolidayRepository(string holidayDirectoryPath, HolidayJsonParser? parser = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(holidayDirectoryPath);

        this.holidayDirectoryPath = holidayDirectoryPath;
        this.parser = parser ?? new HolidayJsonParser();
    }

    public async Task<IReadOnlyList<HolidayDay>> GetHolidayDaysAsync(
        int year,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ValidateYear(year);

        lock (cacheLock)
        {
            if (cache.TryGetValue(year, out IReadOnlyList<HolidayDay>? cachedDays))
            {
                return cachedDays;
            }
        }

        string filePath = Path.Combine(holidayDirectoryPath, FormattableString.Invariant($"{year:D4}.json"));
        if (!File.Exists(filePath))
        {
            throw new HolidayDataException(
                $"Holiday data file for year '{year}' was not found at '{filePath}'.");
        }

        string json = await ReadJsonAsync(filePath, year, cancellationToken).ConfigureAwait(false);
        IReadOnlyList<HolidayDay> parsedDays = parser.Parse(json, year).Days;

        lock (cacheLock)
        {
            if (!cache.TryGetValue(year, out IReadOnlyList<HolidayDay>? cachedDays))
            {
                cache[year] = parsedDays;
                return parsedDays;
            }

            return cachedDays;
        }
    }

    private static async Task<string> ReadJsonAsync(
        string filePath,
        int year,
        CancellationToken cancellationToken)
    {
        try
        {
            return await File.ReadAllTextAsync(filePath, Encoding.UTF8, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw new HolidayDataException($"Holiday data file for year '{year}' could not be read.", exception);
        }
    }

    private static void ValidateYear(int year)
    {
        _ = new CalendarMonth(year, 1);
    }
}
