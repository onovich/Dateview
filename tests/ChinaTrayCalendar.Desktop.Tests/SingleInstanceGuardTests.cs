namespace ChinaTrayCalendar.Desktop.Tests;

public sealed class SingleInstanceGuardTests
{
    [Fact]
    public void TryAcquireReturnsPrimaryWhenNameIsAvailable()
    {
        using SingleInstanceGuard guard = SingleInstanceGuard.TryAcquire(CreateMutexName());

        Assert.True(guard.IsPrimaryInstance);
    }

    [Fact]
    public void TryAcquireReturnsSecondaryWhenNameIsAlreadyHeld()
    {
        string mutexName = CreateMutexName();
        using SingleInstanceGuard firstGuard = SingleInstanceGuard.TryAcquire(mutexName);

        using SingleInstanceGuard secondGuard = SingleInstanceGuard.TryAcquire(mutexName);

        Assert.True(firstGuard.IsPrimaryInstance);
        Assert.False(secondGuard.IsPrimaryInstance);
    }

    [Fact]
    public void DisposeReleasesPrimaryGuard()
    {
        string mutexName = CreateMutexName();
        using (SingleInstanceGuard firstGuard = SingleInstanceGuard.TryAcquire(mutexName))
        {
            Assert.True(firstGuard.IsPrimaryInstance);
        }

        using SingleInstanceGuard secondGuard = SingleInstanceGuard.TryAcquire(mutexName);

        Assert.True(secondGuard.IsPrimaryInstance);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void TryAcquireRejectsBlankName(string mutexName)
    {
        Assert.Throws<ArgumentException>(() => SingleInstanceGuard.TryAcquire(mutexName));
    }

    private static string CreateMutexName()
    {
        return $@"Local\ChinaTrayCalendar.Dateview.Tests.{Guid.NewGuid():N}";
    }
}
