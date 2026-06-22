using System.Runtime.Versioning;
using System.Security;
using ChinaTrayCalendar.Application.Ports;

namespace ChinaTrayCalendar.Infrastructure.Startup;

public sealed class WindowsAutoStartService : IAutoStartService
{
    internal const string RunValueName = "ChinaTrayCalendar";

    private readonly string quotedExecutablePath;
    private readonly IRunRegistryKey runRegistryKey;

    [SupportedOSPlatform("windows")]
    public WindowsAutoStartService(string executablePath)
        : this(executablePath, new CurrentUserRunRegistryKey())
    {
    }

    internal WindowsAutoStartService(string executablePath, IRunRegistryKey runRegistryKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(executablePath);
        ArgumentNullException.ThrowIfNull(runRegistryKey);

        if (!Path.IsPathFullyQualified(executablePath))
        {
            throw new ArgumentException("Executable path must be fully qualified.", nameof(executablePath));
        }

        if (executablePath.Contains('"', StringComparison.Ordinal))
        {
            throw new ArgumentException("Executable path must not contain quote characters.", nameof(executablePath));
        }

        quotedExecutablePath = QuoteExecutablePath(executablePath);
        this.runRegistryKey = runRegistryKey;
    }

    public bool IsEnabled()
    {
        try
        {
            return string.Equals(
                runRegistryKey.GetValue(RunValueName),
                quotedExecutablePath,
                StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception exception) when (IsRegistryException(exception))
        {
            throw new AutoStartRegistrationException("Startup registration could not be read.", exception);
        }
    }

    public void SetEnabled(bool enabled)
    {
        try
        {
            if (enabled)
            {
                runRegistryKey.SetValue(RunValueName, quotedExecutablePath);
                return;
            }

            runRegistryKey.DeleteValue(RunValueName);
        }
        catch (Exception exception) when (IsRegistryException(exception))
        {
            throw new AutoStartRegistrationException("Startup registration could not be updated.", exception);
        }
    }

    internal static string QuoteExecutablePath(string executablePath)
    {
        return $"\"{executablePath}\"";
    }

    private static bool IsRegistryException(Exception exception)
    {
        return exception is IOException or SecurityException or UnauthorizedAccessException;
    }
}
