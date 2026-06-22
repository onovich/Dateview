using ChinaTrayCalendar.Application.Calendars;
using ChinaTrayCalendar.Application.Ports;
using ChinaTrayCalendar.Desktop.ViewModels;
using ChinaTrayCalendar.Domain;

namespace ChinaTrayCalendar.Desktop.Tests;

public sealed class CalendarViewModelTests
{
    [Fact]
    public void CalendarPopupWindowCanBeConstructed()
    {
        Exception? exception = null;
        Thread thread = new(() =>
        {
            try
            {
                CalendarPopupWindow window = new();
                Assert.False(window.ShowInTaskbar);
                Assert.Equal(System.Windows.WindowStyle.None, window.WindowStyle);
                Assert.Equal(System.Windows.ResizeMode.NoResize, window.ResizeMode);
                Assert.Equal(360, window.Width);
                Assert.Equal(420, window.Height);
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
    public void CalendarPopupWindowHidesOnEscape()
    {
        Exception? exception = null;
        bool? isVisibleAfterEscape = null;
        Thread thread = new(() =>
        {
            try
            {
                CalendarPopupWindow window = new();
                window.Show();
                System.Windows.PresentationSource inputSource =
                    System.Windows.PresentationSource.FromDependencyObject(
                        (System.Windows.DependencyObject)(object)window)
                    ?? throw new InvalidOperationException("Popup window has no presentation source.");

                window.RaiseEvent(new System.Windows.Input.KeyEventArgs(
                    System.Windows.Input.Keyboard.PrimaryDevice,
                    inputSource,
                    Environment.TickCount,
                    System.Windows.Input.Key.Escape)
                {
                    RoutedEvent = System.Windows.Input.Keyboard.PreviewKeyDownEvent,
                });
                isVisibleAfterEscape = window.IsVisible;
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
        Assert.False(isVisibleAfterEscape);
    }

    [Fact]
    public async Task LoadAsyncPopulatesCurrentMonth()
    {
        FakeClock clock = new(new DateOnly(2026, 6, 22));
        CalendarViewModel viewModel = CreateViewModel(clock);

        await viewModel.LoadAsync();

        Assert.Equal(new CalendarMonth(2026, 6), viewModel.DisplayedMonth);
        Assert.Equal("2026年6月", viewModel.MonthTitle);
        Assert.Equal(["一", "二", "三", "四", "五", "六", "日"], viewModel.WeekdayHeaders);
        Assert.Equal(MonthGrid.CellCount, viewModel.Cells.Count);
        Assert.False(viewModel.IsLoading);
        Assert.Null(viewModel.ErrorMessage);
    }

    [Fact]
    public async Task PreviousAndNextCommandsNavigateMonths()
    {
        FakeClock clock = new(new DateOnly(2026, 6, 22));
        CalendarViewModel viewModel = CreateViewModel(clock);
        await viewModel.LoadAsync();

        await viewModel.PreviousMonthCommand.ExecuteAsync();
        Assert.Equal(new CalendarMonth(2026, 5), viewModel.DisplayedMonth);

        await viewModel.NextMonthCommand.ExecuteAsync();
        Assert.Equal(new CalendarMonth(2026, 6), viewModel.DisplayedMonth);
    }

    [Fact]
    public async Task TodayCommandReturnsToClockMonth()
    {
        FakeClock clock = new(new DateOnly(2026, 6, 22));
        CalendarViewModel viewModel = CreateViewModel(clock);
        await viewModel.LoadAsync();
        await viewModel.NextMonthCommand.ExecuteAsync();

        await viewModel.TodayCommand.ExecuteAsync();

        Assert.Equal(new CalendarMonth(2026, 6), viewModel.DisplayedMonth);
    }

    [Fact]
    public async Task ApplyFirstDayOfWeekAsyncRefreshesHeadersAndGrid()
    {
        FakeClock clock = new(new DateOnly(2026, 6, 22));
        CalendarViewModel viewModel = CreateViewModel(clock);
        await viewModel.LoadAsync();

        await viewModel.ApplyFirstDayOfWeekAsync(DayOfWeek.Sunday);

        Assert.Equal(["日", "一", "二", "三", "四", "五", "六"], viewModel.WeekdayHeaders);
        Assert.Equal(new DateOnly(2026, 5, 31), viewModel.Cells[0].Date);
    }

    [Fact]
    public void CloseCommandRaisesCloseRequested()
    {
        CalendarViewModel viewModel = CreateViewModel(new FakeClock(new DateOnly(2026, 6, 22)));
        bool closeRequested = false;
        viewModel.CloseRequested += (_, _) => closeRequested = true;

        viewModel.CloseCommand.Execute(parameter: null);

        Assert.True(closeRequested);
    }

    [Fact]
    public async Task LoadAsyncSurfacesErrorState()
    {
        FakeClock clock = new(new DateOnly(2026, 6, 22));
        FailingHolidayRepository holidayRepository = new();
        GetMonthCalendarUseCase useCase = new(clock, holidayRepository);
        CalendarViewModel viewModel = new(useCase, clock);

        await viewModel.LoadAsync();

        Assert.Equal("日历加载失败", viewModel.ErrorMessage);
        Assert.Empty(viewModel.Cells);
        Assert.False(viewModel.IsLoading);
    }

    private static CalendarViewModel CreateViewModel(FakeClock clock)
    {
        FakeHolidayRepository holidayRepository = new();
        holidayRepository.SetYear(2026, []);
        GetMonthCalendarUseCase useCase = new(clock, holidayRepository);

        return new CalendarViewModel(useCase, clock);
    }

    private sealed class FakeClock(DateOnly today) : IClock
    {
        public DateOnly Today { get; } = today;
    }

    private sealed class FakeHolidayRepository : IHolidayRepository
    {
        private readonly Dictionary<int, IReadOnlyList<HolidayDay>> holidaysByYear = [];

        public Task<IReadOnlyList<HolidayDay>> GetHolidayDaysAsync(
            int year,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(holidaysByYear.TryGetValue(year, out IReadOnlyList<HolidayDay>? holidays)
                ? holidays
                : Array.Empty<HolidayDay>());
        }

        public void SetYear(int year, IReadOnlyList<HolidayDay> holidays)
        {
            holidaysByYear[year] = holidays;
        }
    }

    private sealed class FailingHolidayRepository : IHolidayRepository
    {
        public Task<IReadOnlyList<HolidayDay>> GetHolidayDaysAsync(
            int year,
            CancellationToken cancellationToken)
        {
            throw new InvalidOperationException($"Cannot load {year}.");
        }
    }
}
