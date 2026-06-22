namespace ChinaTrayCalendar.Application.Ports;

public interface IAutoStartService
{
    bool IsEnabled();

    void SetEnabled(bool enabled);
}
