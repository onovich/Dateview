using System.Drawing;
using System.IO;
using System.Reflection;

namespace ChinaTrayCalendar.Desktop.Tray;

internal sealed class DateviewTrayIconProvider : ITrayIconProvider
{
    private const string EmbeddedIconResourceName = "DateviewIcon.ico";

    public Icon CreateIcon()
    {
        using Stream? resourceStream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream(EmbeddedIconResourceName);

        if (resourceStream is not null)
        {
            return CreateIconFromStream(resourceStream);
        }

        string? processPath = Environment.ProcessPath;
        if (!string.IsNullOrWhiteSpace(processPath) && File.Exists(processPath))
        {
            Icon? associatedIcon = Icon.ExtractAssociatedIcon(processPath);
            if (associatedIcon is not null)
            {
                return associatedIcon;
            }
        }

        throw new InvalidOperationException("Dateview tray icon resource could not be loaded.");
    }

    private static Icon CreateIconFromStream(Stream stream)
    {
        using MemoryStream copy = new();
        stream.CopyTo(copy);
        copy.Position = 0;

        using Icon icon = new(copy);
        return (Icon)icon.Clone();
    }
}
