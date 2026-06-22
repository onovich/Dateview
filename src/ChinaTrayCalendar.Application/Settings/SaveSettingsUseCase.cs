using ChinaTrayCalendar.Application.Ports;

namespace ChinaTrayCalendar.Application.Settings;

public sealed class SaveSettingsUseCase
{
    private readonly ISettingsStore settingsStore;

    public SaveSettingsUseCase(ISettingsStore settingsStore)
    {
        ArgumentNullException.ThrowIfNull(settingsStore);

        this.settingsStore = settingsStore;
    }

    public Task ExecuteAsync(AppSettings settings, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return settingsStore.SaveAsync(settings, cancellationToken);
    }
}
