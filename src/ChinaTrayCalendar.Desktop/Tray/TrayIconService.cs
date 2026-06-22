using System.Drawing;
using System.Windows.Forms;

namespace ChinaTrayCalendar.Desktop.Tray;

internal sealed class TrayIconService : IDisposable
{
    private readonly ITrayIconFactory trayIconFactory;
    private readonly Func<Point> getCursorPosition;
    private ITrayIcon? trayIcon;

    public TrayIconService(ITrayIconFactory trayIconFactory, Func<Point>? getCursorPosition = null)
    {
        this.trayIconFactory = trayIconFactory ?? throw new ArgumentNullException(nameof(trayIconFactory));
        this.getCursorPosition = getCursorPosition ?? (() => Cursor.Position);
    }

    public event EventHandler<TrayIconPrimaryClickEventArgs>? PrimaryClick;

    public event EventHandler? ExitRequested;

    public event EventHandler? SettingsRequested;

    public event EventHandler? TodayRequested;

    public bool IsVisible => trayIcon?.Visible == true;

    public void Show()
    {
        trayIcon ??= CreateTrayIcon();
        trayIcon.Visible = true;
    }

    public void Dispose()
    {
        if (trayIcon is null)
        {
            return;
        }

        trayIcon.MouseUp -= OnTrayIconMouseUp;
        trayIcon.Visible = false;
        trayIcon.ContextMenuStrip?.Dispose();
        trayIcon.ContextMenuStrip = null;
        trayIcon.Dispose();
        trayIcon = null;
    }

    private ITrayIcon CreateTrayIcon()
    {
        ITrayIcon icon = trayIconFactory.Create();
        icon.Icon = SystemIcons.Application;
        icon.Text = DesktopStrings.AppName;
        icon.ContextMenuStrip = CreateContextMenu();
        icon.MouseUp += OnTrayIconMouseUp;

        return icon;
    }

    private ContextMenuStrip CreateContextMenu()
    {
        ContextMenuStrip menu = new();
        ToolStripMenuItem todayItem = new(DesktopStrings.TrayMenuToday);
        todayItem.Click += (_, _) => TodayRequested?.Invoke(this, EventArgs.Empty);

        ToolStripMenuItem settingsItem = new(DesktopStrings.TrayMenuSettings);
        settingsItem.Click += (_, _) => SettingsRequested?.Invoke(this, EventArgs.Empty);
        ToolStripMenuItem startWithWindowsItem = new(DesktopStrings.TrayMenuStartWithWindows)
        {
            Enabled = false,
        };
        ToolStripMenuItem exitItem = new(DesktopStrings.TrayMenuExit);
        exitItem.Click += (_, _) => ExitRequested?.Invoke(this, EventArgs.Empty);

        menu.Items.Add(todayItem);
        menu.Items.Add(settingsItem);
        menu.Items.Add(startWithWindowsItem);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(exitItem);

        return menu;
    }

    private void OnTrayIconMouseUp(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            PrimaryClick?.Invoke(this, new TrayIconPrimaryClickEventArgs(getCursorPosition()));
        }
    }
}
