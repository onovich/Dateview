namespace ChinaTrayCalendar.Domain;

public sealed class MonthGrid
{
    public const int RowCount = 6;
    public const int ColumnCount = 7;
    public const int CellCount = RowCount * ColumnCount;

    public MonthGrid(CalendarMonth month, IReadOnlyList<CalendarDay> days)
    {
        ArgumentNullException.ThrowIfNull(days);

        if (days.Count != CellCount)
        {
            throw new ArgumentException($"Month grid must contain exactly {CellCount} days.", nameof(days));
        }

        Month = month;
        Days = days.ToArray();
    }

    public CalendarMonth Month { get; }

    public IReadOnlyList<CalendarDay> Days { get; }
}
