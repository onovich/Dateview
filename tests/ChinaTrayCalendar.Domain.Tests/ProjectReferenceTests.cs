using ChinaTrayCalendar.Domain;

namespace ChinaTrayCalendar.Domain.Tests;

public sealed class ProjectReferenceTests
{
    [Fact]
    public void DomainAssemblyIsReferenceable()
    {
        Assert.Equal("ChinaTrayCalendar.Domain", typeof(DomainAssemblyMarker).Assembly.GetName().Name);
    }
}
