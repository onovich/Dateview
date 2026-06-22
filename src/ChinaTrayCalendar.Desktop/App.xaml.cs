using System.IO;
using System.Windows;
using ChinaTrayCalendar.Application.Calendars;
using ChinaTrayCalendar.Application.Ports;
using ChinaTrayCalendar.Application.Settings;
using ChinaTrayCalendar.Application.Startup;
using ChinaTrayCalendar.Desktop.PopupPlacement;
using ChinaTrayCalendar.Desktop.Tray;
using ChinaTrayCalendar.Desktop.ViewModels;
using ChinaTrayCalendar.Infrastructure.Holidays;
using ChinaTrayCalendar.Infrastructure.Settings;
using ChinaTrayCalendar.Infrastructure.Startup;
using ChinaTrayCalendar.Infrastructure.Time;
using DrawingPoint = System.Drawing.Point;

namespace ChinaTrayCalendar.Desktop;

public partial class App : System.Windows.Application
{
    private const string SingleInstanceMutexName = @"Local\ChinaTrayCalendar.Dateview";

    private readonly PopupAnimationService popupAnimationService = new();
    private readonly PopupWindowPlacer popupWindowPlacer = new();
    private IAutoStartService? autoStartService;
    private SingleInstanceGuard? singleInstanceGuard;
    private ISettingsStore? settingsStore;
    private TrayIconService? trayIconService;
    private CalendarPopupWindow? popupWindow;
    private CalendarViewModel? calendarViewModel;
    private Window? settingsWindow;

    protected override async void OnStartup(StartupEventArgs e)
    {
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        base.OnStartup(e);

        singleInstanceGuard = SingleInstanceGuard.TryAcquire(SingleInstanceMutexName);
        if (!singleInstanceGuard.IsPrimaryInstance)
        {
            Shutdown(exitCode: 0);
            return;
        }

        trayIconService = new TrayIconService(new NotifyIconFactory());
        trayIconService.ExitRequested += OnTrayExitRequested;
        trayIconService.PrimaryClick += OnTrayIconPrimaryClick;
        trayIconService.SettingsRequested += OnTraySettingsRequested;
        trayIconService.TodayRequested += OnTrayTodayRequested;
        trayIconService.Show();

        settingsStore = JsonSettingsStore.CreateDefault();
        autoStartService = new WindowsAutoStartService(GetExecutablePath());
        AppSettings settings = await LoadSettingsOrDefaultAsync(settingsStore);

        popupWindow = new CalendarPopupWindow();
        calendarViewModel = CreateCalendarViewModel(settings.FirstDayOfWeek);
        calendarViewModel.CloseRequested += (_, _) => HidePopup();
        popupWindow.DataContext = calendarViewModel;
        MainWindow = popupWindow;

        await calendarViewModel.LoadAsync();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (trayIconService is not null)
        {
            trayIconService.ExitRequested -= OnTrayExitRequested;
            trayIconService.PrimaryClick -= OnTrayIconPrimaryClick;
            trayIconService.SettingsRequested -= OnTraySettingsRequested;
            trayIconService.TodayRequested -= OnTrayTodayRequested;
            trayIconService.Dispose();
        }

        settingsWindow?.Close();
        popupWindow?.Close();
        singleInstanceGuard?.Dispose();
        base.OnExit(e);
    }

    private void OnTrayExitRequested(object? sender, EventArgs e)
    {
        Shutdown(exitCode: 0);
    }

    private void OnTrayIconPrimaryClick(object? sender, TrayIconPrimaryClickEventArgs e)
    {
        TogglePopup(e.ScreenPoint);
    }

    private async void OnTraySettingsRequested(object? sender, EventArgs e)
    {
        if (settingsWindow?.IsVisible == true)
        {
            settingsWindow.Activate();
            return;
        }

        if (settingsStore is null || autoStartService is null)
        {
            return;
        }

        SettingsViewModel viewModel = new(
            new LoadSettingsUseCase(settingsStore),
            new SaveSettingsUseCase(settingsStore),
            new ToggleStartupUseCase(autoStartService),
            autoStartService);
        Window window = (Window)(object)new SettingsWindow();
        window.DataContext = viewModel;
        window.Owner = popupWindow?.IsVisible == true ? (Window)(object)popupWindow : null;
        settingsWindow = window;

        viewModel.CloseRequested += (_, _) => window.Close();
        viewModel.SettingsSaved += OnSettingsSaved;
        window.Closed += (_, _) =>
        {
            viewModel.SettingsSaved -= OnSettingsSaved;
            settingsWindow = null;
        };

        await viewModel.LoadCommand.ExecuteAsync();
        window.Show();
    }

    private async void OnTrayTodayRequested(object? sender, EventArgs e)
    {
        if (calendarViewModel is not null)
        {
            await calendarViewModel.TodayCommand.ExecuteAsync();
        }
    }

    private async void OnSettingsSaved(object? sender, AppSettings settings)
    {
        if (calendarViewModel is not null)
        {
            await calendarViewModel.ApplyFirstDayOfWeekAsync(settings.FirstDayOfWeek);
        }
    }

    private void TogglePopup(DrawingPoint clickPoint)
    {
        if (popupWindow is null)
        {
            return;
        }

        if (popupWindow.IsVisible)
        {
            HidePopup();
            return;
        }

        popupWindowPlacer.Place(popupWindow, clickPoint);
        popupWindow.Show();
        popupAnimationService.PlayEntrance((Window)(object)popupWindow);
    }

    private void HidePopup()
    {
        popupWindow?.Hide();
    }

    private static CalendarViewModel CreateCalendarViewModel(DayOfWeek firstDayOfWeek)
    {
        SystemClock clock = new();
        string holidayDirectoryPath = Path.Combine(AppContext.BaseDirectory, "assets", "holidays", "cn");
        JsonHolidayRepository holidayRepository = new(holidayDirectoryPath);
        GetMonthCalendarUseCase useCase = new(clock, holidayRepository);

        return new CalendarViewModel(useCase, clock, firstDayOfWeek);
    }

    private static async Task<AppSettings> LoadSettingsOrDefaultAsync(ISettingsStore settingsStore)
    {
        try
        {
            return await settingsStore.LoadAsync(CancellationToken.None);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return AppSettings.CreateDefault();
        }
    }

    private static string GetExecutablePath()
    {
        return Environment.ProcessPath
            ?? throw new InvalidOperationException("Current executable path could not be resolved.");
    }
}
