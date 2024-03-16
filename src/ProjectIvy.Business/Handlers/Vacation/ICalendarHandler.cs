using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Calendar;
using ProjectIvy.Model.View.Calendar;

namespace ProjectIvy.Business.Handlers.Vendor
{
    public interface ICalendarHandler : IHandler
    {
        Task CreateVacation(DateTime date);

        Task DeleteVacation(DateTime date);

        Task<CalendarSection> Get(DateTime from, DateTime to);

        Task UpdateDay(DateTime day, CalendarDayUpdateBinding b);
    }
}
