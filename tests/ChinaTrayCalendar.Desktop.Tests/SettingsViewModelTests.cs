using ChinaTrayCalendar.Application.Ports;
using ChinaTrayCalendar.Application.Settings;
using ChinaTrayCalendar.Application.Startup;
using ChinaTrayCalendar.Desktop.ViewModels;

namespace ChinaTrayCalendar.Desktop.Tests;

public sealed class SettingsViewModelTests
{
    [Fact]
    public void SettingsWindowCanShowWithViewModel()
    {
        Exception? exception = null;
        Thread thread = new(() =>
        {
            try
            {
                FakeSettingsStore settingsStore = new(AppSettings.CreateDefault());
                FakeAutoStartService autoStartService = new();
                SettingsViewModel viewModel = CreateViewModel(settingsStore, autoStartService);
                viewModel.LoadCommand.ExecuteAsync().GetAwaiter().GetResult();
                SettingsWindow window = new()
                {
                    DataContext = viewModel,
                };

                window.Show();
                window.UpdateLayout();

                Assert.False(window.ShowInTaskbar);
                Assert.Equal("设置", window.Title);
                window.Close();
            }
            catch (Exception caughtException)
            {
                exception = caughtException;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);

        thread.Start();
        thread.Join();

        Assert.Null(exception);
    }

    [Fact]
    public async Task LoadCommandPopulatesSettingsAndObservedStartupState()
    {
        FakeSettingsStore settingsStore = new(new AppSettings(DayOfWeek.Sunday, startWithWindows: false));
        FakeAutoStartService autoStartService = new() { Enabled = true };
        SettingsViewModel viewModel = CreateViewModel(settingsStore, autoStartService);

        await viewModel.LoadCommand.ExecuteAsync();

        Assert.Equal(DayOfWeek.Sunday, viewModel.SelectedFirstDayOfWeek.Value);
        Assert.True(viewModel.StartWithWindows);
        Assert.Null(viewModel.ErrorMessage);
    }

    [Fact]
    public async Task SaveCommandPersistsSettingsTogglesStartupAndRequestsClose()
    {
        FakeSettingsStore settingsStore = new(AppSettings.CreateDefault());
        FakeAutoStartService autoStartService = new();
        SettingsViewModel viewModel = CreateViewModel(settingsStore, autoStartService);
        await viewModel.LoadCommand.ExecuteAsync();
        viewModel.SelectedFirstDayOfWeek = viewModel.FirstDayOptions.Single(option => option.Value == DayOfWeek.Sunday);
        viewModel.StartWithWindows = true;
        AppSettings? savedEventSettings = null;
        bool closeRequested = false;
        viewModel.SettingsSaved += (_, settings) => savedEventSettings = settings;
        viewModel.CloseRequested += (_, _) => closeRequested = true;

        await viewModel.SaveCommand.ExecuteAsync();

        AppSettings expected = new(DayOfWeek.Sunday, startWithWindows: true);
        Assert.Equal(expected, settingsStore.SavedSettings);
        Assert.Equal(expected, savedEventSettings);
        Assert.True(autoStartService.Enabled);
        Assert.True(closeRequested);
        Assert.Null(viewModel.ErrorMessage);
    }

    [Fact]
    public async Task SaveCommandSurfacesFailureWithoutClosing()
    {
        FakeSettingsStore settingsStore = new(AppSettings.CreateDefault())
        {
            ThrowOnSave = true,
        };
        FakeAutoStartService autoStartService = new();
        SettingsViewModel viewModel = CreateViewModel(settingsStore, autoStartService);
        bool closeRequested = false;
        viewModel.CloseRequested += (_, _) => closeRequested = true;

        await viewModel.SaveCommand.ExecuteAsync();

        Assert.False(closeRequested);
        Assert.NotNull(viewModel.ErrorMessage);
    }

    private static SettingsViewModel CreateViewModel(
        FakeSettingsStore settingsStore,
        FakeAutoStartService autoStartService)
    {
        return new SettingsViewModel(
            new LoadSettingsUseCase(settingsStore),
            new SaveSettingsUseCase(settingsStore),
            new ToggleStartupUseCase(autoStartService),
            autoStartService);
    }

    private sealed class FakeSettingsStore(AppSettings settings) : ISettingsStore
    {
        public AppSettings? SavedSettings { get; private set; }

        public bool ThrowOnSave { get; init; }

        public Task<AppSettings> LoadAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(settings);
        }

        public Task SaveAsync(AppSettings settings, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (ThrowOnSave)
            {
                throw new InvalidOperationException("Save failed.");
            }

            SavedSettings = settings;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeAutoStartService : IAutoStartService
    {
        public bool Enabled { get; set; }

        public bool IsEnabled()
        {
            return Enabled;
        }

        public void SetEnabled(bool enabled)
        {
            Enabled = enabled;
        }
    }
}
