using System.ComponentModel;
using System.Runtime.CompilerServices;
using ChinaTrayCalendar.Application.Ports;
using ChinaTrayCalendar.Application.Settings;
using ChinaTrayCalendar.Application.Startup;

namespace ChinaTrayCalendar.Desktop.ViewModels;

public sealed class SettingsViewModel : INotifyPropertyChanged
{
    private readonly IAutoStartService autoStartService;
    private readonly LoadSettingsUseCase loadSettingsUseCase;
    private readonly SaveSettingsUseCase saveSettingsUseCase;
    private readonly ToggleStartupUseCase toggleStartupUseCase;
    private string? errorMessage;
    private bool isBusy;
    private DayOfWeekOption selectedFirstDayOfWeek;
    private bool startWithWindows;
    private AppTheme theme = AppTheme.System;

    public SettingsViewModel(
        LoadSettingsUseCase loadSettingsUseCase,
        SaveSettingsUseCase saveSettingsUseCase,
        ToggleStartupUseCase toggleStartupUseCase,
        IAutoStartService autoStartService)
    {
        ArgumentNullException.ThrowIfNull(loadSettingsUseCase);
        ArgumentNullException.ThrowIfNull(saveSettingsUseCase);
        ArgumentNullException.ThrowIfNull(toggleStartupUseCase);
        ArgumentNullException.ThrowIfNull(autoStartService);

        this.loadSettingsUseCase = loadSettingsUseCase;
        this.saveSettingsUseCase = saveSettingsUseCase;
        this.toggleStartupUseCase = toggleStartupUseCase;
        this.autoStartService = autoStartService;

        FirstDayOptions = CreateFirstDayOptions();
        selectedFirstDayOfWeek = FirstDayOptions[0];
        LoadCommand = new AsyncRelayCommand(LoadAsync, () => !IsBusy);
        SaveCommand = new AsyncRelayCommand(SaveAsync, () => !IsBusy);
        CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(this, EventArgs.Empty));
    }

    public event EventHandler? CloseRequested;

    public event EventHandler<AppSettings>? SettingsSaved;

    public event PropertyChangedEventHandler? PropertyChanged;

    public IReadOnlyList<DayOfWeekOption> FirstDayOptions { get; }

    public DayOfWeekOption SelectedFirstDayOfWeek
    {
        get => selectedFirstDayOfWeek;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            SetProperty(ref selectedFirstDayOfWeek, value);
        }
    }

    public bool StartWithWindows
    {
        get => startWithWindows;
        set => SetProperty(ref startWithWindows, value);
    }

    public string ThemeLabel => DesktopStrings.SettingsThemeSystem;

    public string? ErrorMessage
    {
        get => errorMessage;
        private set => SetProperty(ref errorMessage, value);
    }

    public bool IsBusy
    {
        get => isBusy;
        private set
        {
            if (SetProperty(ref isBusy, value))
            {
                LoadCommand.RaiseCanExecuteChanged();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public AsyncRelayCommand LoadCommand { get; }

    public AsyncRelayCommand SaveCommand { get; }

    public RelayCommand CancelCommand { get; }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            AppSettings settings = await loadSettingsUseCase.ExecuteAsync(cancellationToken).ConfigureAwait(true);
            Apply(settings);
            StartWithWindows = autoStartService.IsEnabled();
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            ErrorMessage = "设置加载失败";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveAsync(CancellationToken cancellationToken)
    {
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            bool observedStartWithWindows = toggleStartupUseCase.Execute(StartWithWindows);
            AppSettings settings = new(SelectedFirstDayOfWeek.Value, observedStartWithWindows, theme);
            await saveSettingsUseCase.ExecuteAsync(settings, cancellationToken).ConfigureAwait(true);
            SettingsSaved?.Invoke(this, settings);
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            ErrorMessage = "设置保存失败";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void Apply(AppSettings settings)
    {
        SelectedFirstDayOfWeek = FirstDayOptions.First(option => option.Value == settings.FirstDayOfWeek);
        StartWithWindows = settings.StartWithWindows;
        theme = settings.Theme;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ThemeLabel)));
    }

    private static DayOfWeekOption[] CreateFirstDayOptions()
    {
        return
        [
            new DayOfWeekOption(DayOfWeek.Monday, "星期一"),
            new DayOfWeekOption(DayOfWeek.Sunday, "星期日"),
            new DayOfWeekOption(DayOfWeek.Saturday, "星期六"),
        ];
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        return true;
    }
}
