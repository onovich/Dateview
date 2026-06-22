using System.Drawing;

namespace ChinaTrayCalendar.Desktop.Tray;

internal sealed class TrayIconPrimaryClickEventArgs(Point screenPoint) : EventArgs
{
    public Point ScreenPoint { get; } = screenPoint;
}
