using ChinaTrayCalendar.Domain;

namespace ChinaTrayCalendar.Application.Ports;

public interface IHolidayRepository
{
    Task<IReadOnlyList<HolidayDay>> GetHolidayDaysAsync(int year, CancellationToken cancellationToken);
}
