using System.Drawing;
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
        public Icon? Icon { get; set; } = SystemIcons.Application;

        public string Text { get; set; } = string.Empty;

        public bool Visible { get; set; }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
