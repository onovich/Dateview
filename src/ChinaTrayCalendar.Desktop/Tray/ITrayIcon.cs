using System.Drawing;

namespace ChinaTrayCalendar.Desktop.Tray;

internal interface ITrayIcon : IDisposable
{
    Icon? Icon { get; set; }

    string Text { get; set; }

    bool Visible { get; set; }
}
