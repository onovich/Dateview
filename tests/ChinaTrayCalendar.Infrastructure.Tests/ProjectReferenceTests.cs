using ChinaTrayCalendar.Infrastructure;

namespace ChinaTrayCalendar.Infrastructure.Tests;

public sealed class ProjectReferenceTests
{
    [Fact]
    public void InfrastructureAssemblyIsReferenceable()
    {
        Assert.Equal("ChinaTrayCalendar.Infrastructure", typeof(InfrastructureAssemblyMarker).Assembly.GetName().Name);
    }
}
