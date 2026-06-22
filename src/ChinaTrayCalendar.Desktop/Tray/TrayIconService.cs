using System.Drawing;
using System.Windows.Forms;

namespace ChinaTrayCalendar.Desktop.Tray;

internal sealed class TrayIconService(ITrayIconFactory trayIconFactory) : IDisposable
{
    private const string TooltipText = "Dateview";

    private ITrayIcon? trayIcon;

    public event EventHandler? PrimaryClick;

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

        trayIcon.MouseUp -= OnTrayIconMouseUp;
        trayIcon.Visible = false;
        trayIcon.Dispose();
        trayIcon = null;
    }

    private ITrayIcon CreateTrayIcon()
    {
        ITrayIcon icon = trayIconFactory.Create();
        icon.Icon = SystemIcons.Application;
        icon.Text = TooltipText;
        icon.MouseUp += OnTrayIconMouseUp;

        return icon;
    }

    private void OnTrayIconMouseUp(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            PrimaryClick?.Invoke(this, EventArgs.Empty);
        }
    }
}
