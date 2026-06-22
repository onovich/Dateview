namespace ChinaTrayCalendar.Desktop;

internal sealed class SingleInstanceGuard : IDisposable
{
    private readonly Mutex? mutex;

    private SingleInstanceGuard(Mutex? mutex, bool isPrimaryInstance)
    {
        this.mutex = mutex;
        IsPrimaryInstance = isPrimaryInstance;
    }

    public bool IsPrimaryInstance { get; }

    public static SingleInstanceGuard TryAcquire(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Single instance mutex name must not be blank.", nameof(name));
        }

        Mutex mutex = new(initiallyOwned: true, name, out bool createdNew);
        if (createdNew)
        {
            return new SingleInstanceGuard(mutex, isPrimaryInstance: true);
        }

        mutex.Dispose();
        return new SingleInstanceGuard(mutex: null, isPrimaryInstance: false);
    }

    public void Dispose()
    {
        if (mutex is null)
        {
            return;
        }

        mutex.ReleaseMutex();
        mutex.Dispose();
    }
}
