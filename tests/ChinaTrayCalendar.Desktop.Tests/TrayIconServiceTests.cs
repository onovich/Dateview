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
        Assert.Equal("Dateview", icon.Text);
        Assert.NotNull(icon.Icon);
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

        service.Dispose();

        FakeTrayIcon icon = Assert.Single(factory.CreatedIcons);
        Assert.False(icon.Visible);
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
