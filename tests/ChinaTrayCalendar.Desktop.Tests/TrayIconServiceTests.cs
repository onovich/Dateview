using System.Drawing;
using System.Windows.Forms;
using ChinaTrayCalendar.Desktop.Tray;

namespace ChinaTrayCalendar.Desktop.Tests;

public sealed class TrayIconServiceTests
{
    [Fact]
    public void ShowCreatesAndDisplaysTrayIcon()
    {
        FakeTrayIconFactory factory = new();
        using TrayIconService service = new(factory);

        service.Show();

        FakeTrayIcon icon = Assert.Single(factory.CreatedIcons);
        Assert.True(icon.Visible);
        Assert.Equal(DesktopStrings.AppName, icon.Text);
        Assert.NotNull(icon.Icon);
        Assert.NotNull(icon.ContextMenuStrip);
        Assert.True(service.IsVisible);
    }

    [Fact]
    public void ShowReusesExistingTrayIcon()
    {
        FakeTrayIconFactory factory = new();
        using TrayIconService service = new(factory);

        service.Show();
        service.Show();

        Assert.Single(factory.CreatedIcons);
    }

    [Fact]
    public void DisposeHidesAndDisposesTrayIcon()
    {
        FakeTrayIconFactory factory = new();
        TrayIconService service = new(factory);
        service.Show();
        ContextMenuStrip? contextMenu = Assert.Single(factory.CreatedIcons).ContextMenuStrip;

        service.Dispose();

        FakeTrayIcon icon = Assert.Single(factory.CreatedIcons);
        Assert.False(icon.Visible);
        Assert.Null(icon.ContextMenuStrip);
        Assert.True(contextMenu?.IsDisposed);
        Assert.True(icon.IsDisposed);
        Assert.False(service.IsVisible);
    }

    [Fact]
    public void DisposeCanRunBeforeShow()
    {
        FakeTrayIconFactory factory = new();
        TrayIconService service = new(factory);

        service.Dispose();

        Assert.Empty(factory.CreatedIcons);
        Assert.False(service.IsVisible);
    }

    [Fact]
    public void LeftMouseUpRaisesPrimaryClick()
    {
        FakeTrayIconFactory factory = new();
        using TrayIconService service = new(factory, () => new Point(120, 840));
        int clickCount = 0;
        Point? screenPoint = null;
        service.PrimaryClick += (_, e) =>
        {
            clickCount++;
            screenPoint = e.ScreenPoint;
        };
        service.Show();

        Assert.Single(factory.CreatedIcons).RaiseMouseUp(MouseButtons.Left);

        Assert.Equal(1, clickCount);
        Assert.Equal(new Point(120, 840), screenPoint);
    }

    [Fact]
    public void RightMouseUpDoesNotRaisePrimaryClick()
    {
        FakeTrayIconFactory factory = new();
        using TrayIconService service = new(factory);
        int clickCount = 0;
        service.PrimaryClick += (_, _) => clickCount++;
        service.Show();

        Assert.Single(factory.CreatedIcons).RaiseMouseUp(MouseButtons.Right);

        Assert.Equal(0, clickCount);
    }

    [Fact]
    public void DisposeUnsubscribesMouseEvents()
    {
        FakeTrayIconFactory factory = new();
        TrayIconService service = new(factory);
        int clickCount = 0;
        service.PrimaryClick += (_, _) => clickCount++;
        service.Show();
        FakeTrayIcon icon = Assert.Single(factory.CreatedIcons);

        service.Dispose();
        icon.RaiseMouseUp(MouseButtons.Left);

        Assert.Equal(0, clickCount);
    }

    [Fact]
    public void ShowCreatesBasicContextMenuItems()
    {
        FakeTrayIconFactory factory = new();
        using TrayIconService service = new(factory);

        service.Show();

        ContextMenuStrip menu = Assert.Single(factory.CreatedIcons).ContextMenuStrip
            ?? throw new InvalidOperationException("Context menu was not created.");
        Assert.Equal(5, menu.Items.Count);
        AssertMenuItem(menu, index: 0, DesktopStrings.TrayMenuToday, enabled: true);
        AssertMenuItem(menu, index: 1, DesktopStrings.TrayMenuSettings, enabled: true);
        AssertMenuItem(menu, index: 2, DesktopStrings.TrayMenuStartWithWindows, enabled: false);
        Assert.IsType<ToolStripSeparator>(menu.Items[3]);
        AssertMenuItem(menu, index: 4, DesktopStrings.TrayMenuExit, enabled: true);
    }

    [Fact]
    public void TodayMenuClickRaisesTodayRequested()
    {
        FakeTrayIconFactory factory = new();
        using TrayIconService service = new(factory);
        int requestCount = 0;
        service.TodayRequested += (_, _) => requestCount++;
        service.Show();

        ToolStripMenuItem todayItem = GetMenuItem(factory, index: 0);
        todayItem.PerformClick();

        Assert.Equal(1, requestCount);
    }

    [Fact]
    public void SettingsMenuClickRaisesSettingsRequested()
    {
        FakeTrayIconFactory factory = new();
        using TrayIconService service = new(factory);
        int requestCount = 0;
        service.SettingsRequested += (_, _) => requestCount++;
        service.Show();

        ToolStripMenuItem settingsItem = GetMenuItem(factory, index: 1);
        settingsItem.PerformClick();

        Assert.Equal(1, requestCount);
    }

    [Fact]
    public void ExitMenuClickRaisesExitRequested()
    {
        FakeTrayIconFactory factory = new();
        using TrayIconService service = new(factory);
        int requestCount = 0;
        service.ExitRequested += (_, _) => requestCount++;
        service.Show();

        ToolStripMenuItem exitItem = GetMenuItem(factory, index: 4);
        exitItem.PerformClick();

        Assert.Equal(1, requestCount);
    }

    private static void AssertMenuItem(ContextMenuStrip menu, int index, string expectedText, bool enabled)
    {
        ToolStripMenuItem item = Assert.IsType<ToolStripMenuItem>(menu.Items[index]);
        Assert.Equal(expectedText, item.Text);
        Assert.Equal(enabled, item.Enabled);
    }

    private static ToolStripMenuItem GetMenuItem(FakeTrayIconFactory factory, int index)
    {
        ContextMenuStrip menu = Assert.Single(factory.CreatedIcons).ContextMenuStrip
            ?? throw new InvalidOperationException("Context menu was not created.");

        return Assert.IsType<ToolStripMenuItem>(menu.Items[index]);
    }

    private sealed class FakeTrayIconFactory : ITrayIconFactory
    {
        public List<FakeTrayIcon> CreatedIcons { get; } = [];

        public ITrayIcon Create()
        {
            FakeTrayIcon icon = new();
            CreatedIcons.Add(icon);

            return icon;
        }
    }

    private sealed class FakeTrayIcon : ITrayIcon
    {
        public event MouseEventHandler? MouseUp;

        public Icon? Icon { get; set; } = SystemIcons.Application;

        public ContextMenuStrip? ContextMenuStrip { get; set; }

        public string Text { get; set; } = string.Empty;

        public bool Visible { get; set; }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }

        public void RaiseMouseUp(MouseButtons mouseButton)
        {
            MouseUp?.Invoke(this, new MouseEventArgs(mouseButton, clicks: 1, x: 0, y: 0, delta: 0));
        }
    }
}
