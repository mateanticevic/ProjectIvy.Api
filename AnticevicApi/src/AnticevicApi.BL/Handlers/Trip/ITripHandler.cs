using AnticevicApi.Model.Binding.Trip;
using AnticevicApi.Model.View;
using View = AnticevicApi.Model.View.Trip;

namespace AnticevicApi.BL.Handlers.Trip
{
    public interface ITripHandler
    {
        void AddCityToTrip(string tripValueId, string cityValueId);

        PaginatedView<View.Trip> Get(TripGetBinding binding);

        View.Trip GetSingle(string valueId);
    }
}