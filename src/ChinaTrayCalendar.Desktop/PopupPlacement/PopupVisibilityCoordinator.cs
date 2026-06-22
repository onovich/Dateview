using DrawingPoint = System.Drawing.Point;

namespace ChinaTrayCalendar.Desktop.PopupPlacement;

internal sealed class PopupVisibilityCoordinator
{
    private DrawingPoint? pendingReopenPoint;

    public bool IsClosing { get; private set; }

    public PopupToggleAction HandleTrayClick(bool isVisible, DrawingPoint clickPoint)
    {
        if (IsClosing)
        {
            pendingReopenPoint = clickPoint;
            return PopupToggleAction.None;
        }

        if (isVisible)
        {
            IsClosing = true;
            pendingReopenPoint = null;
            return PopupToggleAction.Close;
        }

        return PopupToggleAction.Open;
    }

    public bool TryBeginClose(bool isVisible)
    {
        if (!isVisible || IsClosing)
        {
            return false;
        }

        IsClosing = true;
        pendingReopenPoint = null;

        return true;
    }

    public DrawingPoint? CompleteClose()
    {
        DrawingPoint? reopenPoint = pendingReopenPoint;
        pendingReopenPoint = null;
        IsClosing = false;

        return reopenPoint;
    }
}
