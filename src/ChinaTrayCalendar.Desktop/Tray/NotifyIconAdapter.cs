using System.Drawing;
using System.Windows.Forms;

namespace ChinaTrayCalendar.Desktop.Tray;

internal sealed class NotifyIconAdapter(NotifyIcon notifyIcon) : ITrayIcon
{
    public event MouseEventHandler? MouseUp
    {
        add => notifyIcon.MouseUp += value;
        remove => notifyIcon.MouseUp -= value;
    }

    public Icon? Icon
    {
        get => notifyIcon.Icon;
        set => notifyIcon.Icon = value;
    }

    public string Text
    {
        get => notifyIcon.Text;
        set => notifyIcon.Text = value;
    }

    public bool Visible
    {
        get => notifyIcon.Visible;
        set => notifyIcon.Visible = value;
    }

    public void Dispose()
    {
        notifyIcon.Dispose();
    }
}
