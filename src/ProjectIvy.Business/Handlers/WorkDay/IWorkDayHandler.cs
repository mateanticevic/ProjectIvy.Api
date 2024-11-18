using System.Threading.Tasks;
using ProjectIvy.Model.Binding;

namespace ProjectIvy.Business.Handlers.WorkDay;

public interface IWorkDayHandler
{
    Task<IEnumerable<Model.View.WorkDay.WorkDay>> Get(FilteredBinding binding);
}
