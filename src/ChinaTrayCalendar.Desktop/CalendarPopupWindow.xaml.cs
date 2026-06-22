using System.Windows.Threading;
using InputKey = System.Windows.Input.Key;
using InputKeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace ChinaTrayCalendar.Desktop;

public partial class CalendarPopupWindow
{
    private readonly DispatcherTimer deactivationSuppressionTimer;
    private bool suppressDeactivation;

    public CalendarPopupWindow()
    {
        InitializeComponent();

        deactivationSuppressionTimer = new DispatcherTimer(
            TimeSpan.FromMilliseconds(250),
            DispatcherPriority.Normal,
            OnDeactivationSuppressionTimerTick,
            Dispatcher);
        deactivationSuppressionTimer.Stop();
    }

    public void SuppressDeactivationForTrayOpen()
    {
        suppressDeactivation = true;
        deactivationSuppressionTimer.Stop();
        deactivationSuppressionTimer.Start();
    }

    private void OnDeactivated(object? sender, EventArgs e)
    {
        if (suppressDeactivation)
        {
            return;
        }

        Hide();
    }

    private void OnPreviewKeyDown(object sender, InputKeyEventArgs e)
    {
        if (e.Key != InputKey.Escape)
        {
            return;
        }

        Hide();
        e.Handled = true;
    }

    private void OnDeactivationSuppressionTimerTick(object? sender, EventArgs e)
    {
        suppressDeactivation = false;
        deactivationSuppressionTimer.Stop();
    }
}
