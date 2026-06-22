using System.Drawing;
using System.Windows.Forms;

namespace ChinaTrayCalendar.Desktop.Tray;

internal interface ITrayIcon : IDisposable
{
    event MouseEventHandler? MouseUp;

    Icon? Icon { get; set; }

    string Text { get; set; }

    bool Visible { get; set; }
}
