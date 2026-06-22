using ChinaTrayCalendar.Domain;

namespace ChinaTrayCalendar.Domain.Tests;

public sealed class HolidaySourceTests
{
    [Fact]
    public void ConstructorStoresSourceMetadata()
    {
        DateOnly publishedDate = new(2025, 11, 4);

        HolidaySource source = new("Official notice", publishedDate, "https://example.invalid/notice");

        Assert.Equal("Official notice", source.Title);
        Assert.Equal(publishedDate, source.PublishedDate);
        Assert.Equal("https://example.invalid/notice", source.Url);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ConstructorRejectsBlankTitle(string title)
    {
        Assert.Throws<ArgumentException>(() => new HolidaySource(title, new DateOnly(2025, 11, 4)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ConstructorRejectsBlankUrlWhenProvided(string url)
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => new HolidaySource("Official notice", new DateOnly(2025, 11, 4), url));

        Assert.Equal("url", exception.ParamName);
    }
}
