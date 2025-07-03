using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Trip;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Trip;

namespace ProjectIvy.Business.Handlers.Trip;

public interface ITripHandler
{
    void AddCity(string tripValueId, string cityValueId);

    void AddExpense(string tripValueId, string expenseValueId);

    void AddPoi(string tripValueId, string poiValueId);

    void Create(TripBinding binding);

    Task<IEnumerable<KeyValuePair<int, int>>> DaysByYear(TripGetBinding binding);

    void Delete(string valueId);

    PagedView<View.Trip> Get(TripGetBinding binding);

    View.Trip GetSingle(string valueId);

    void RemoveCity(string tripValueId, string cityValueId);

    void RemoveExpense(string tripValueId, string expenseValueId);

    void RemovePoi(string tripValueId, string poiValueId);
}
