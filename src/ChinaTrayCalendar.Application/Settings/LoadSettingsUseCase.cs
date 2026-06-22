using ChinaTrayCalendar.Application.Ports;

namespace ChinaTrayCalendar.Application.Settings;

public sealed class LoadSettingsUseCase
{
    private readonly ISettingsStore settingsStore;

    public LoadSettingsUseCase(ISettingsStore settingsStore)
    {
        ArgumentNullException.ThrowIfNull(settingsStore);

        this.settingsStore = settingsStore;
    }

    public Task<AppSettings> ExecuteAsync(CancellationToken cancellationToken)
    {
        return settingsStore.LoadAsync(cancellationToken);
    }
}
