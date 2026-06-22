using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media;

namespace ChinaTrayCalendar.Desktop.PopupPlacement;

internal sealed class PopupWindowPlacer
{
    private const double MarginDip = 8;

    public void Place(CalendarPopupWindow window, Point clickPoint)
    {
        ArgumentNullException.ThrowIfNull(window);

        System.Windows.DpiScale dpi = VisualTreeHelper.GetDpi((Visual)(object)window);
        Screen screen = Screen.FromPoint(clickPoint);
        Size popupSize = new(
            (int)Math.Ceiling(window.Width * dpi.DpiScaleX),
            (int)Math.Ceiling(window.Height * dpi.DpiScaleY));
        int margin = (int)Math.Ceiling(MarginDip * Math.Max(dpi.DpiScaleX, dpi.DpiScaleY));

        Point location = PopupPlacementCalculator.Calculate(
            screen.Bounds,
            screen.WorkingArea,
            clickPoint,
            popupSize,
            margin);

        window.Left = location.X / dpi.DpiScaleX;
        window.Top = location.Y / dpi.DpiScaleY;
    }
}
