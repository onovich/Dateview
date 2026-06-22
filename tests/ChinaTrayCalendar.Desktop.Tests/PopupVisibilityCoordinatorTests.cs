using System.Drawing;
using ChinaTrayCalendar.Desktop.PopupPlacement;

namespace ChinaTrayCalendar.Desktop.Tests;

public sealed class PopupVisibilityCoordinatorTests
{
    [Fact]
    public void TrayClickWhenHiddenOpensPopup()
    {
        PopupVisibilityCoordinator coordinator = new();

        PopupToggleAction action = coordinator.HandleTrayClick(isVisible: false, new Point(10, 20));

        Assert.Equal(PopupToggleAction.Open, action);
        Assert.False(coordinator.IsClosing);
    }

    [Fact]
    public void TrayClickWhenVisibleStartsClose()
    {
        PopupVisibilityCoordinator coordinator = new();

        PopupToggleAction action = coordinator.HandleTrayClick(isVisible: true, new Point(10, 20));

        Assert.Equal(PopupToggleAction.Close, action);
        Assert.True(coordinator.IsClosing);
    }

    [Fact]
    public void TrayClickDuringCloseQueuesLatestReopenPoint()
    {
        PopupVisibilityCoordinator coordinator = new();
        Assert.Equal(PopupToggleAction.Close, coordinator.HandleTrayClick(isVisible: true, new Point(10, 20)));

        Assert.Equal(PopupToggleAction.None, coordinator.HandleTrayClick(isVisible: true, new Point(30, 40)));
        Assert.Equal(PopupToggleAction.None, coordinator.HandleTrayClick(isVisible: true, new Point(50, 60)));

        Assert.Equal(new Point(50, 60), coordinator.CompleteClose());
        Assert.False(coordinator.IsClosing);
    }

    [Fact]
    public void DuplicateCloseRequestDuringCloseIsIgnored()
    {
        PopupVisibilityCoordinator coordinator = new();
        Assert.True(coordinator.TryBeginClose(isVisible: true));

        Assert.False(coordinator.TryBeginClose(isVisible: true));

        Assert.Null(coordinator.CompleteClose());
        Assert.False(coordinator.IsClosing);
    }
}
