using InputKey = System.Windows.Input.Key;
using InputKeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace ChinaTrayCalendar.Desktop;

public partial class CalendarPopupWindow
{
    public CalendarPopupWindow()
    {
        InitializeComponent();
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
}
