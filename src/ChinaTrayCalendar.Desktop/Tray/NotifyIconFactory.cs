using System.Windows.Forms;

namespace ChinaTrayCalendar.Desktop.Tray;

internal sealed class NotifyIconFactory : ITrayIconFactory
{
    public ITrayIcon Create()
    {
        return new NotifyIconAdapter(new NotifyIcon());
    }
}
