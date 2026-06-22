using ChinaTrayCalendar.Application.Ports;
using ChinaTrayCalendar.Application.Startup;

namespace ChinaTrayCalendar.Application.Tests;

public sealed class ToggleStartupUseCaseTests
{
    [Fact]
    public void ExecuteEnablesStartupAndReturnsObservedState()
    {
        FakeAutoStartService autoStartService = new();
        ToggleStartupUseCase useCase = new(autoStartService);

        bool enabled = useCase.Execute(enabled: true);

        Assert.True(enabled);
        Assert.True(autoStartService.Enabled);
    }

    [Fact]
    public void ExecuteDisablesStartupAndReturnsObservedState()
    {
        FakeAutoStartService autoStartService = new() { Enabled = true };
        ToggleStartupUseCase useCase = new(autoStartService);

        bool enabled = useCase.Execute(enabled: false);

        Assert.False(enabled);
        Assert.False(autoStartService.Enabled);
    }

    private sealed class FakeAutoStartService : IAutoStartService
    {
        public bool Enabled { get; set; }

        public bool IsEnabled()
        {
            return Enabled;
        }

        public void SetEnabled(bool enabled)
        {
            Enabled = enabled;
        }
    }
}
