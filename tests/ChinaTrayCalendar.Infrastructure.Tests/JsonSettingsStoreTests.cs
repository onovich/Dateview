using ChinaTrayCalendar.Application.Settings;
using ChinaTrayCalendar.Infrastructure.Settings;

namespace ChinaTrayCalendar.Infrastructure.Tests;

public sealed class JsonSettingsStoreTests
{
    [Fact]
    public void ConstructorRejectsBlankPath()
    {
        Assert.Throws<ArgumentException>(() => new JsonSettingsStore("   "));
    }

    [Fact]
    public void GetDefaultFilePathUsesRoamingAppData()
    {
        string expectedDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ChinaTrayCalendar");

        string filePath = JsonSettingsStore.GetDefaultFilePath();

        Assert.Equal(Path.Combine(expectedDirectory, "settings.json"), filePath);
    }

    [Fact]
    public async Task LoadAsyncReturnsDefaultWhenFileIsMissing()
    {
        using TemporarySettingsDirectory directory = new();
        JsonSettingsStore store = new(directory.GetSettingsPath());

        AppSettings settings = await store.LoadAsync(CancellationToken.None);

        Assert.Equal(AppSettings.CreateDefault(), settings);
    }

    [Fact]
    public async Task SaveAsyncAndLoadAsyncRoundTripSettings()
    {
        using TemporarySettingsDirectory directory = new();
        JsonSettingsStore store = new(directory.GetSettingsPath());
        AppSettings expected = new(DayOfWeek.Sunday, startWithWindows: true, AppTheme.Dark);

        await store.SaveAsync(expected, CancellationToken.None);
        AppSettings actual = await store.LoadAsync(CancellationToken.None);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task SaveAsyncCreatesContainingDirectory()
    {
        using TemporarySettingsDirectory directory = new();
        string settingsPath = Path.Combine(directory.DirectoryPath, "nested", "settings.json");
        JsonSettingsStore store = new(settingsPath);

        await store.SaveAsync(AppSettings.CreateDefault(), CancellationToken.None);

        Assert.True(File.Exists(settingsPath));
    }

    [Fact]
    public async Task SaveAsyncReplacesExistingFile()
    {
        using TemporarySettingsDirectory directory = new();
        string settingsPath = directory.GetSettingsPath();
        await File.WriteAllTextAsync(settingsPath, """{"firstDayOfWeek":"Monday"}""");
        JsonSettingsStore store = new(settingsPath);
        AppSettings expected = new(DayOfWeek.Saturday, startWithWindows: true, AppTheme.Light);

        await store.SaveAsync(expected, CancellationToken.None);
        AppSettings actual = await store.LoadAsync(CancellationToken.None);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task LoadAsyncThrowsForCorruptJson()
    {
        using TemporarySettingsDirectory directory = new();
        string settingsPath = directory.GetSettingsPath();
        await File.WriteAllTextAsync(settingsPath, "{not-json");
        JsonSettingsStore store = new(settingsPath);

        SettingsDataException exception = await Assert.ThrowsAsync<SettingsDataException>(
            () => store.LoadAsync(CancellationToken.None));

        Assert.Contains("invalid", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task LoadAsyncThrowsForInvalidEnumValue()
    {
        using TemporarySettingsDirectory directory = new();
        string settingsPath = directory.GetSettingsPath();
        await File.WriteAllTextAsync(settingsPath, """{"firstDayOfWeek":"Moonday"}""");
        JsonSettingsStore store = new(settingsPath);

        SettingsDataException exception = await Assert.ThrowsAsync<SettingsDataException>(
            () => store.LoadAsync(CancellationToken.None));

        Assert.Contains("invalid", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task SaveAsyncRejectsNullSettings()
    {
        using TemporarySettingsDirectory directory = new();
        JsonSettingsStore store = new(directory.GetSettingsPath());

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => store.SaveAsync(settings: null!, CancellationToken.None));
    }

    private sealed class TemporarySettingsDirectory : IDisposable
    {
        public TemporarySettingsDirectory()
        {
            DirectoryPath = Path.Combine(
                Path.GetTempPath(),
                "ChinaTrayCalendarSettingsTests",
                Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(DirectoryPath);
        }

        public string DirectoryPath { get; }

        public string GetSettingsPath()
        {
            return Path.Combine(DirectoryPath, "settings.json");
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
