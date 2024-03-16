using System.Threading.Tasks;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Calendar;
using ProjectIvy.Model.View;

namespace ProjectIvy.Business.Handlers.Vendor
{
    public interface ICalendarHandler : IHandler
    {
        Task CreateVacation(DateTime date);

        Task DeleteVacation(DateTime date);

        Task<PagedView<DateTime>> Get(FilteredPagedBinding binding);

        Task UpdateDay(DateTime day, CalendarDayUpdateBinding b);
    }
}
