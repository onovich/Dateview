using ChinaTrayCalendar.Application.Ports;

namespace ChinaTrayCalendar.Infrastructure.Time;

public sealed class SystemClock : IClock
{
    public DateOnly Today => DateOnly.FromDateTime(DateTime.Now);
}
