namespace ChinaTrayCalendar.Application.Ports;

public interface IClock
{
    DateOnly Today { get; }
}
