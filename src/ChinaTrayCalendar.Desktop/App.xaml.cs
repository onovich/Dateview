using System.IO;
using System.Windows;
using ChinaTrayCalendar.Application.Calendars;
using ChinaTrayCalendar.Desktop.PopupPlacement;
using ChinaTrayCalendar.Desktop.Tray;
using ChinaTrayCalendar.Desktop.ViewModels;
using ChinaTrayCalendar.Infrastructure.Holidays;
using ChinaTrayCalendar.Infrastructure.Time;
using DrawingPoint = System.Drawing.Point;

namespace ChinaTrayCalendar.Desktop;

public partial class App : System.Windows.Application
{
    private const string SingleInstanceMutexName = @"Local\ChinaTrayCalendar.Dateview";

    private readonly PopupAnimationService popupAnimationService = new();
    private readonly PopupWindowPlacer popupWindowPlacer = new();
    private SingleInstanceGuard? singleInstanceGuard;
    private TrayIconService? trayIconService;
    private CalendarPopupWindow? popupWindow;

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
        trayIconService.PrimaryClick += OnTrayIconPrimaryClick;
        trayIconService.Show();

        popupWindow = new CalendarPopupWindow();
        CalendarViewModel viewModel = CreateCalendarViewModel();
        viewModel.CloseRequested += (_, _) => HidePopup();
        popupWindow.DataContext = viewModel;
        MainWindow = popupWindow;

        await viewModel.LoadAsync();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (trayIconService is not null)
        {
            trayIconService.PrimaryClick -= OnTrayIconPrimaryClick;
            trayIconService.Dispose();
        }

        popupWindow?.Close();
        singleInstanceGuard?.Dispose();
        base.OnExit(e);
    }

    private void OnTrayIconPrimaryClick(object? sender, TrayIconPrimaryClickEventArgs e)
    {
        TogglePopup(e.ScreenPoint);
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

    private static CalendarViewModel CreateCalendarViewModel()
    {
        SystemClock clock = new();
        string holidayDirectoryPath = Path.Combine(AppContext.BaseDirectory, "assets", "holidays", "cn");
        JsonHolidayRepository holidayRepository = new(holidayDirectoryPath);
        GetMonthCalendarUseCase useCase = new(clock, holidayRepository);

        return new CalendarViewModel(useCase, clock);
    }
}
