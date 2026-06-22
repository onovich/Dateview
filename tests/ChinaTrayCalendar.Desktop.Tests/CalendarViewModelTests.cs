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
    public void CloseCommandRaisesCloseRequested()
    {
        CalendarViewModel viewModel = CreateViewModel(new FakeClock(new DateOnly(2026, 6, 22)));
        bool closeRequested = false;
        viewModel.CloseRequested += (_, _) => closeRequested = true;

        viewModel.CloseCommand.Execute(parameter: null);

        Assert.True(closeRequested);
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
}
