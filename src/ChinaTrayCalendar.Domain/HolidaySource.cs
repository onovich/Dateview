namespace ChinaTrayCalendar.Domain;

public sealed record HolidaySource
{
    public HolidaySource(string title, DateOnly publishedDate, string? url = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        if (url is not null && string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("Source URL must not be blank when provided.", nameof(url));
        }

        Title = title;
        PublishedDate = publishedDate;
        Url = url;
    }

    public string Title { get; }

    public DateOnly PublishedDate { get; }

    public string? Url { get; }
}
