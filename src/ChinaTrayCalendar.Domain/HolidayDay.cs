namespace ChinaTrayCalendar.Domain;

public sealed record HolidayDay
{
    public HolidayDay(DateOnly date, HolidayDayType type, string name)
    {
        if (!Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(nameof(type), type, "Holiday day type is not supported.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Date = date;
        Type = type;
        Name = name;
    }

    public DateOnly Date { get; }

    public HolidayDayType Type { get; }

    public string Name { get; }
}
