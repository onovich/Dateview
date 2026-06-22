using System.ComponentModel;
using System.Runtime.CompilerServices;
using ChinaTrayCalendar.Application.Calendars;
using ChinaTrayCalendar.Application.Ports;
using ChinaTrayCalendar.Domain;

namespace ChinaTrayCalendar.Desktop.ViewModels;

public sealed class CalendarViewModel : INotifyPropertyChanged
{
    private readonly IClock clock;
    private readonly DayOfWeek firstDayOfWeek;
    private readonly GetMonthCalendarUseCase getMonthCalendarUseCase;
    private IReadOnlyList<CalendarDayDto> cells = [];
    private CalendarMonth displayedMonth;
    private string? errorMessage;
    private bool isLoading;
    private string monthTitle = string.Empty;

    public CalendarViewModel(
        GetMonthCalendarUseCase getMonthCalendarUseCase,
        IClock clock,
        DayOfWeek firstDayOfWeek = DayOfWeek.Monday)
    {
        ArgumentNullException.ThrowIfNull(getMonthCalendarUseCase);
        ArgumentNullException.ThrowIfNull(clock);

        this.getMonthCalendarUseCase = getMonthCalendarUseCase;
        this.clock = clock;
        this.firstDayOfWeek = firstDayOfWeek;

        displayedMonth = new CalendarMonth(clock.Today.Year, clock.Today.Month);
        MonthTitle = FormatMonthTitle(displayedMonth);
        WeekdayHeaders = BuildWeekdayHeaders(firstDayOfWeek);
        PreviousMonthCommand = new AsyncRelayCommand(
            cancellationToken => LoadMonthAsync(DisplayedMonth.AddMonths(-1), cancellationToken),
            () => !IsLoading);
        NextMonthCommand = new AsyncRelayCommand(
            cancellationToken => LoadMonthAsync(DisplayedMonth.AddMonths(1), cancellationToken),
            () => !IsLoading);
        TodayCommand = new AsyncRelayCommand(
            cancellationToken =>
            {
                DateOnly today = clock.Today;
                return LoadMonthAsync(new CalendarMonth(today.Year, today.Month), cancellationToken);
            },
            () => !IsLoading);
        CloseCommand = new RelayCommand(() => CloseRequested?.Invoke(this, EventArgs.Empty));
    }

    public event EventHandler? CloseRequested;

    public event PropertyChangedEventHandler? PropertyChanged;

    public CalendarMonth DisplayedMonth
    {
        get => displayedMonth;
        private set
        {
            if (SetProperty(ref displayedMonth, value))
            {
                MonthTitle = FormatMonthTitle(value);
            }
        }
    }

    public string MonthTitle
    {
        get => monthTitle;
        private set => SetProperty(ref monthTitle, value);
    }

    public IReadOnlyList<string> WeekdayHeaders { get; }

    public IReadOnlyList<CalendarDayDto> Cells
    {
        get => cells;
        private set => SetProperty(ref cells, value);
    }

    public bool IsLoading
    {
        get => isLoading;
        private set
        {
            if (SetProperty(ref isLoading, value))
            {
                PreviousMonthCommand.RaiseCanExecuteChanged();
                NextMonthCommand.RaiseCanExecuteChanged();
                TodayCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string? ErrorMessage
    {
        get => errorMessage;
        private set => SetProperty(ref errorMessage, value);
    }

    public AsyncRelayCommand PreviousMonthCommand { get; }

    public AsyncRelayCommand NextMonthCommand { get; }

    public AsyncRelayCommand TodayCommand { get; }

    public RelayCommand CloseCommand { get; }

    public Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadMonthAsync(DisplayedMonth, cancellationToken);
    }

    private async Task LoadMonthAsync(CalendarMonth month, CancellationToken cancellationToken)
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            MonthCalendarDto calendar = await getMonthCalendarUseCase
                .ExecuteAsync(month, firstDayOfWeek, cancellationToken)
                .ConfigureAwait(false);
            DisplayedMonth = calendar.Month;
            Cells = calendar.Days;
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            ErrorMessage = "日历加载失败";
            Cells = [];
        }
        finally
        {
            IsLoading = false;
        }
    }

    private static IReadOnlyList<string> BuildWeekdayHeaders(DayOfWeek firstDayOfWeek)
    {
        string[] labels = ["日", "一", "二", "三", "四", "五", "六"];

        return Enumerable
            .Range(0, 7)
            .Select(offset => labels[((int)firstDayOfWeek + offset) % 7])
            .ToArray();
    }

    private static string FormatMonthTitle(CalendarMonth month)
    {
        return FormattableString.Invariant($"{month.Year:D4}年{month.Month}月");
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
