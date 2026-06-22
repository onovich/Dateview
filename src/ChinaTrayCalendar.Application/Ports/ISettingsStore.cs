using ChinaTrayCalendar.Application.Settings;

namespace ChinaTrayCalendar.Application.Ports;

public interface ISettingsStore
{
    Task<AppSettings> LoadAsync(CancellationToken cancellationToken);

    Task SaveAsync(AppSettings settings, CancellationToken cancellationToken);
}
