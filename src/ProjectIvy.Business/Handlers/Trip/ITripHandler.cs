using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Trip;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Trip;

namespace ProjectIvy.Business.Handlers.Trip;

public interface ITripHandler
{
    Task AddCity(string tripValueId, string cityValueId);

    Task AddExpense(string tripValueId, string expenseValueId);

    Task AddPoi(string tripValueId, string poiValueId);

    Task Create(TripBinding binding);

    Task<IEnumerable<KeyValuePair<int, int>>> DaysByYear(TripGetBinding binding);

    Task Delete(string valueId);

    Task<PagedView<View.Trip>> Get(TripGetBinding binding);

    Task<View.Trip> GetSingle(string valueId);

    Task RemoveCity(string tripValueId, string cityValueId);

    Task RemoveExpense(string tripValueId, string expenseValueId);

    Task RemovePoi(string tripValueId, string poiValueId);
}
