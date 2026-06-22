namespace ChinaTrayCalendar.Domain;

public readonly record struct CalendarMonth
{
    public const int MinYear = 1900;
    public const int MaxYear = 2100;

    public CalendarMonth(int year, int month)
    {
        if (year is < MinYear or > MaxYear)
        {
            throw new ArgumentOutOfRangeException(
                nameof(year),
                year,
                $"Year must be between {MinYear} and {MaxYear}.");
        }

        if (month is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(
                nameof(month),
                month,
                "Month must be between 1 and 12.");
        }

        Year = year;
        Month = month;
    }

    public int Year { get; }

    public int Month { get; }

    public DateOnly FirstDay => new(Year, Month, 1);

    public int DaysInMonth => DateTime.DaysInMonth(Year, Month);

    public CalendarMonth AddMonths(int months)
    {
        DateOnly target = FirstDay.AddMonths(months);

        return new CalendarMonth(target.Year, target.Month);
    }

    public override string ToString()
    {
        return FormattableString.Invariant($"{Year:D4}-{Month:D2}");
    }
}
