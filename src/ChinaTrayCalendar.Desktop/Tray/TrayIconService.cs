using System.Drawing;
using System.Windows.Forms;

namespace ChinaTrayCalendar.Desktop.Tray;

internal sealed class TrayIconService : IDisposable
{
    private const string TooltipText = "Dateview";

    private readonly ITrayIconFactory trayIconFactory;
    private readonly Func<Point> getCursorPosition;
    private ITrayIcon? trayIcon;

    public TrayIconService(ITrayIconFactory trayIconFactory, Func<Point>? getCursorPosition = null)
    {
        this.trayIconFactory = trayIconFactory ?? throw new ArgumentNullException(nameof(trayIconFactory));
        this.getCursorPosition = getCursorPosition ?? (() => Cursor.Position);
    }

    public event EventHandler<TrayIconPrimaryClickEventArgs>? PrimaryClick;

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
            PrimaryClick?.Invoke(this, new TrayIconPrimaryClickEventArgs(getCursorPosition()));
        }
    }
}
