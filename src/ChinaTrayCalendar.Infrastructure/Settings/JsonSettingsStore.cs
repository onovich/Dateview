using System.Text.Json;
using System.Text.Json.Serialization;
using ChinaTrayCalendar.Application.Ports;
using ChinaTrayCalendar.Application.Settings;

namespace ChinaTrayCalendar.Infrastructure.Settings;

public sealed class JsonSettingsStore : ISettingsStore
{
    private const string AppDirectoryName = "ChinaTrayCalendar";
    private const string FileName = "settings.json";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter(namingPolicy: null, allowIntegerValues: false),
        },
    };

    private readonly string filePath;

    public JsonSettingsStore(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        this.filePath = filePath;
    }

    public static JsonSettingsStore CreateDefault()
    {
        return new JsonSettingsStore(GetDefaultFilePath());
    }

    public static string GetDefaultFilePath()
    {
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        return Path.Combine(appDataPath, AppDirectoryName, FileName);
    }

    public async Task<AppSettings> LoadAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!File.Exists(filePath))
        {
            return AppSettings.CreateDefault();
        }

        string json;
        try
        {
            json = await File.ReadAllTextAsync(filePath, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw new SettingsDataException("Settings file could not be read.", exception);
        }

        try
        {
            SettingsFileDto dto = JsonSerializer.Deserialize<SettingsFileDto>(json, SerializerOptions)
                ?? throw new SettingsDataException("Settings JSON must contain an object.");

            return dto.ToAppSettings();
        }
        catch (JsonException exception)
        {
            throw new SettingsDataException("Settings JSON is invalid.", exception);
        }
        catch (ArgumentException exception)
        {
            throw new SettingsDataException("Settings values are invalid.", exception);
        }
    }

    public async Task SaveAsync(AppSettings settings, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(settings);
        cancellationToken.ThrowIfCancellationRequested();

        string? directoryPath = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        SettingsFileDto dto = SettingsFileDto.FromAppSettings(settings);
        string json = JsonSerializer.Serialize(dto, SerializerOptions);
        string temporaryFilePath = $"{filePath}.{Guid.NewGuid():N}.tmp";

        try
        {
            await File.WriteAllTextAsync(temporaryFilePath, json, cancellationToken).ConfigureAwait(false);
            File.Move(temporaryFilePath, filePath, overwrite: true);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw new SettingsDataException("Settings file could not be saved.", exception);
        }
    }

    private sealed class SettingsFileDto
    {
        public DayOfWeek? FirstDayOfWeek { get; init; }

        public bool? StartWithWindows { get; init; }

        public AppTheme? Theme { get; init; }

        public static SettingsFileDto FromAppSettings(AppSettings settings)
        {
            return new SettingsFileDto
            {
                FirstDayOfWeek = settings.FirstDayOfWeek,
                StartWithWindows = settings.StartWithWindows,
                Theme = settings.Theme,
            };
        }

        public AppSettings ToAppSettings()
        {
            return new AppSettings(
                FirstDayOfWeek ?? DayOfWeek.Monday,
                StartWithWindows ?? false,
                Theme ?? AppTheme.System);
        }
    }
}
