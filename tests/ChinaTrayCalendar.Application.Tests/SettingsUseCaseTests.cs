using ChinaTrayCalendar.Application.Ports;
using ChinaTrayCalendar.Application.Settings;

namespace ChinaTrayCalendar.Application.Tests;

public sealed class SettingsUseCaseTests
{
    [Fact]
    public async Task LoadSettingsUseCaseReturnsStoreSettings()
    {
        AppSettings expected = new(DayOfWeek.Sunday, startWithWindows: true, AppTheme.Dark);
        FakeSettingsStore settingsStore = new(expected);
        LoadSettingsUseCase useCase = new(settingsStore);

        AppSettings actual = await useCase.ExecuteAsync(CancellationToken.None);

        Assert.Equal(expected, actual);
        Assert.True(settingsStore.LoadWasCalled);
    }

    [Fact]
    public async Task LoadSettingsUseCaseCanReturnDefaultSettingsFromStore()
    {
        AppSettings expected = AppSettings.CreateDefault();
        FakeSettingsStore settingsStore = new(expected);
        LoadSettingsUseCase useCase = new(settingsStore);

        AppSettings actual = await useCase.ExecuteAsync(CancellationToken.None);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task SaveSettingsUseCasePersistsSettings()
    {
        FakeSettingsStore settingsStore = new(AppSettings.CreateDefault());
        SaveSettingsUseCase useCase = new(settingsStore);
        AppSettings expected = new(DayOfWeek.Sunday, startWithWindows: true, AppTheme.Light);

        await useCase.ExecuteAsync(expected, CancellationToken.None);

        Assert.Equal(expected, settingsStore.SavedSettings);
    }

    [Fact]
    public async Task SaveSettingsUseCaseRejectsNullSettings()
    {
        SaveSettingsUseCase useCase = new(new FakeSettingsStore(AppSettings.CreateDefault()));

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => useCase.ExecuteAsync(settings: null!, CancellationToken.None));
    }

    private sealed class FakeSettingsStore(AppSettings initialSettings) : ISettingsStore
    {
        private readonly AppSettings settings = initialSettings;

        public bool LoadWasCalled { get; private set; }

        public AppSettings? SavedSettings { get; private set; }

        public Task<AppSettings> LoadAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            LoadWasCalled = true;

            return Task.FromResult(settings);
        }

        public Task SaveAsync(AppSettings settings, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            SavedSettings = settings;

            return Task.CompletedTask;
        }
    }
}
