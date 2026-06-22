using System.Drawing;

namespace ChinaTrayCalendar.Desktop.Tray;

internal sealed class TrayIconService(ITrayIconFactory trayIconFactory) : IDisposable
{
    private const string TooltipText = "Dateview";

    private ITrayIcon? trayIcon;

    public bool IsVisible => trayIcon?.Visible == true;

    public void Show()
    {
        trayIcon ??= CreateTrayIcon();
        trayIcon.Visible = true;
    }

    public void Dispose()
    {
        if (trayIcon is null)
        {
            return;
        }

        trayIcon.Visible = false;
        trayIcon.Dispose();
        trayIcon = null;
    }

    private ITrayIcon CreateTrayIcon()
    {
        ITrayIcon icon = trayIconFactory.Create();
        icon.Icon = SystemIcons.Application;
        icon.Text = TooltipText;

        return icon;
    }
}
