using System.IO;
using System.Windows;
using ChinaTrayCalendar.Application.Calendars;
using ChinaTrayCalendar.Desktop.Tray;
using ChinaTrayCalendar.Desktop.ViewModels;
using ChinaTrayCalendar.Infrastructure.Holidays;
using ChinaTrayCalendar.Infrastructure.Time;

namespace ChinaTrayCalendar.Desktop;

public partial class App : System.Windows.Application
{
    private TrayIconService? trayIconService;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        trayIconService = new TrayIconService(new NotifyIconFactory());
        trayIconService.Show();

        CalendarPopupWindow window = new();
        CalendarViewModel viewModel = CreateCalendarViewModel();
        viewModel.CloseRequested += (_, _) => window.Close();
        window.DataContext = viewModel;
        MainWindow = window;
        window.Show();

        await viewModel.LoadAsync();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        trayIconService?.Dispose();
        base.OnExit(e);
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
