using ChinaTrayCalendar.Application;

namespace ChinaTrayCalendar.Application.Tests;

public sealed class ProjectReferenceTests
{
    [Fact]
    public void ApplicationAssemblyIsReferenceable()
    {
        Assert.Equal("ChinaTrayCalendar.Application", typeof(ApplicationAssemblyMarker).Assembly.GetName().Name);
    }
}
