using System.Drawing;

namespace ChinaTrayCalendar.Desktop.PopupPlacement;

internal static class PopupPlacementCalculator
{
    public static Point Calculate(
        Rectangle screenBounds,
        Rectangle workingArea,
        Point clickPoint,
        Size popupSize,
        int margin)
    {
        if (popupSize.Width <= 0 || popupSize.Height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(popupSize), "Popup size must be positive.");
        }

        if (margin < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(margin), "Margin must not be negative.");
        }

        TaskbarEdge edge = DetectTaskbarEdge(screenBounds, workingArea);

        return edge switch
        {
            TaskbarEdge.Left => new Point(
                workingArea.Left + margin,
                Clamp(clickPoint.Y - (popupSize.Height / 2), workingArea.Top + margin, workingArea.Bottom - popupSize.Height - margin)),
            TaskbarEdge.Top => new Point(
                Clamp(clickPoint.X - (popupSize.Width / 2), workingArea.Left + margin, workingArea.Right - popupSize.Width - margin),
                workingArea.Top + margin),
            TaskbarEdge.Right => new Point(
                workingArea.Right - popupSize.Width - margin,
                Clamp(clickPoint.Y - (popupSize.Height / 2), workingArea.Top + margin, workingArea.Bottom - popupSize.Height - margin)),
            TaskbarEdge.Bottom => new Point(
                Clamp(clickPoint.X - (popupSize.Width / 2), workingArea.Left + margin, workingArea.Right - popupSize.Width - margin),
                workingArea.Bottom - popupSize.Height - margin),
            _ => new Point(
                Clamp(clickPoint.X - (popupSize.Width / 2), workingArea.Left + margin, workingArea.Right - popupSize.Width - margin),
                Clamp(clickPoint.Y - popupSize.Height - margin, workingArea.Top + margin, workingArea.Bottom - popupSize.Height - margin)),
        };
    }

    private static TaskbarEdge DetectTaskbarEdge(Rectangle screenBounds, Rectangle workingArea)
    {
        if (workingArea.Left > screenBounds.Left)
        {
            return TaskbarEdge.Left;
        }

        if (workingArea.Top > screenBounds.Top)
        {
            return TaskbarEdge.Top;
        }

        if (workingArea.Right < screenBounds.Right)
        {
            return TaskbarEdge.Right;
        }

        if (workingArea.Bottom < screenBounds.Bottom)
        {
            return TaskbarEdge.Bottom;
        }

        return TaskbarEdge.None;
    }

    private static int Clamp(int value, int minimum, int maximum)
    {
        if (maximum < minimum)
        {
            return minimum;
        }

        return Math.Min(Math.Max(value, minimum), maximum);
    }
}
