using System.IO;
using System.Windows;
using ChinaTrayCalendar.Application.Calendars;
using ChinaTrayCalendar.Desktop.ViewModels;
using ChinaTrayCalendar.Infrastructure.Holidays;
using ChinaTrayCalendar.Infrastructure.Time;

namespace ChinaTrayCalendar.Desktop;

public partial class App : System.Windows.Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        CalendarPopupWindow window = new();
        CalendarViewModel viewModel = CreateCalendarViewModel();
        viewModel.CloseRequested += (_, _) => window.Close();
        window.DataContext = viewModel;
        MainWindow = window;
        window.Show();

        await viewModel.LoadAsync();
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
