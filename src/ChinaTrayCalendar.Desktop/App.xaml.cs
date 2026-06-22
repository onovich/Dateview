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
using FormsCursor = System.Windows.Forms.Cursor;

namespace ChinaTrayCalendar.Desktop;

public partial class App : System.Windows.Application
{
    private const string SingleInstanceMutexName = @"Local\ChinaTrayCalendar.Dateview";

    private readonly PopupAnimationService popupAnimationService = new();
    private readonly PopupVisibilityCoordinator popupVisibilityCoordinator = new();
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
        trayIconService.StartWithWindowsToggleRequested += OnTrayStartWithWindowsToggleRequested;
        trayIconService.TodayRequested += OnTrayTodayRequested;
        trayIconService.Show();

        settingsStore = JsonSettingsStore.CreateDefault();
        autoStartService = new WindowsAutoStartService(GetExecutablePath());
        RefreshTrayStartWithWindowsState();
        AppSettings settings = await LoadSettingsOrDefaultAsync(settingsStore);

        popupWindow = new CalendarPopupWindow();
        calendarViewModel = CreateCalendarViewModel(settings.FirstDayOfWeek);
        calendarViewModel.CloseRequested += (_, _) => BeginHidePopup();
        popupWindow.DismissRequested += (_, _) => BeginHidePopup();
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
            trayIconService.StartWithWindowsToggleRequested -= OnTrayStartWithWindowsToggleRequested;
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

        if (popupWindow is null)
        {
            return;
        }

        if (!popupWindow.IsVisible)
        {
            ShowPopup(FormsCursor.Position);
            return;
        }

        popupWindow.Activate();
    }

    private void OnTrayStartWithWindowsToggleRequested(object? sender, EventArgs e)
    {
        if (autoStartService is null || trayIconService is null)
        {
            return;
        }

        try
        {
            ToggleStartupUseCase toggleStartupUseCase = new(autoStartService);
            bool requestedState = !autoStartService.IsEnabled();
            bool observedState = toggleStartupUseCase.Execute(requestedState);
            trayIconService.SetStartWithWindowsState(observedState);
        }
        catch (AutoStartRegistrationException)
        {
            trayIconService.SetStartWithWindowsState(isChecked: false, isEnabled: false);
            System.Windows.MessageBox.Show(
                "开机启动设置失败",
                DesktopStrings.AppName,
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }

    private async void OnSettingsSaved(object? sender, AppSettings settings)
    {
        if (calendarViewModel is not null)
        {
            await calendarViewModel.ApplyFirstDayOfWeekAsync(settings.FirstDayOfWeek);
        }

        trayIconService?.SetStartWithWindowsState(settings.StartWithWindows);
    }

    private void TogglePopup(DrawingPoint clickPoint)
    {
        if (popupWindow is null)
        {
            return;
        }

        switch (popupVisibilityCoordinator.HandleTrayClick(popupWindow.IsVisible, clickPoint))
        {
            case PopupToggleAction.Open:
                ShowPopup(clickPoint);
                break;
            case PopupToggleAction.Close:
                _ = HidePopupAsync(closeAlreadyStarted: true);
                break;
        }
    }

    private void ShowPopup(DrawingPoint clickPoint)
    {
        if (popupWindow is null)
        {
            return;
        }

        popupWindowPlacer.Place(popupWindow, clickPoint);
        popupWindow.SuppressDeactivationForTrayOpen();
        popupWindow.Show();
        popupWindow.Activate();
        popupAnimationService.PlayEntrance((Window)(object)popupWindow);
    }

    private void BeginHidePopup()
    {
        _ = HidePopupAsync(closeAlreadyStarted: false);
    }

    private async Task HidePopupAsync(bool closeAlreadyStarted)
    {
        if (popupWindow is null)
        {
            return;
        }

        if (!closeAlreadyStarted && !popupVisibilityCoordinator.TryBeginClose(popupWindow.IsVisible))
        {
            return;
        }

        try
        {
            await popupAnimationService.PlayExitAsync((Window)(object)popupWindow);
        }
        finally
        {
            if (popupWindow.IsVisible)
            {
                popupWindow.Hide();
            }

            DrawingPoint? reopenPoint = popupVisibilityCoordinator.CompleteClose();
            if (reopenPoint is not null)
            {
                ShowPopup(reopenPoint.Value);
            }
        }
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

    private void RefreshTrayStartWithWindowsState()
    {
        if (autoStartService is null || trayIconService is null)
        {
            return;
        }

        try
        {
            trayIconService.SetStartWithWindowsState(autoStartService.IsEnabled());
        }
        catch (AutoStartRegistrationException)
        {
            trayIconService.SetStartWithWindowsState(isChecked: false, isEnabled: false);
        }
    }

    private static string GetExecutablePath()
    {
        return Environment.ProcessPath
            ?? throw new InvalidOperationException("Current executable path could not be resolved.");
    }
}
