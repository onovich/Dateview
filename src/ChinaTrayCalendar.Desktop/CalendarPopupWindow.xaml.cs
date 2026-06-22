using System.Windows.Threading;
using InputKey = System.Windows.Input.Key;
using InputKeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace ChinaTrayCalendar.Desktop;

public partial class CalendarPopupWindow
{
    private readonly DispatcherTimer deactivationDismissTimer;
    private readonly DispatcherTimer deactivationSuppressionTimer;
    private bool suppressDeactivation;

    public CalendarPopupWindow()
    {
        InitializeComponent();

        deactivationDismissTimer = new DispatcherTimer(
            TimeSpan.FromMilliseconds(90),
            DispatcherPriority.Normal,
            OnDeactivationDismissTimerTick,
            Dispatcher);
        deactivationDismissTimer.Stop();

        deactivationSuppressionTimer = new DispatcherTimer(
            TimeSpan.FromMilliseconds(250),
            DispatcherPriority.Normal,
            OnDeactivationSuppressionTimerTick,
            Dispatcher);
        deactivationSuppressionTimer.Stop();
    }

    public event EventHandler? DismissRequested;

    public void SuppressDeactivationForTrayOpen()
    {
        suppressDeactivation = true;
        deactivationDismissTimer.Stop();
        deactivationSuppressionTimer.Stop();
        deactivationSuppressionTimer.Start();
    }

    private void OnDeactivated(object? sender, EventArgs e)
    {
        if (suppressDeactivation)
        {
            return;
        }

        deactivationDismissTimer.Stop();
        deactivationDismissTimer.Start();
    }

    private void OnPreviewKeyDown(object sender, InputKeyEventArgs e)
    {
        if (e.Key != InputKey.Escape)
        {
            return;
        }

        RequestDismiss();
        e.Handled = true;
    }

    private void OnDeactivationDismissTimerTick(object? sender, EventArgs e)
    {
        deactivationDismissTimer.Stop();
        RequestDismiss();
    }

    private void OnDeactivationSuppressionTimerTick(object? sender, EventArgs e)
    {
        suppressDeactivation = false;
        deactivationSuppressionTimer.Stop();
    }

    private void RequestDismiss()
    {
        deactivationDismissTimer.Stop();
        DismissRequested?.Invoke(this, EventArgs.Empty);
    }
}
