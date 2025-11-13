using System.Threading.Tasks;
using ProjectIvy.Model.View.Calendar;

namespace ProjectIvy.Business.Services.Calendar;

public interface IIcsCalendarService
{
    Task<IEnumerable<IcsCalendarEvent>> GetEventsAsync(string icsUrl, DateTime from, DateTime to);
}
