using ChinaTrayCalendar.Application.Ports;

namespace ChinaTrayCalendar.Application.Startup;

public sealed class ToggleStartupUseCase
{
    private readonly IAutoStartService autoStartService;

    public ToggleStartupUseCase(IAutoStartService autoStartService)
    {
        ArgumentNullException.ThrowIfNull(autoStartService);

        this.autoStartService = autoStartService;
    }

    public bool Execute(bool enabled)
    {
        autoStartService.SetEnabled(enabled);

        return autoStartService.IsEnabled();
    }
}
