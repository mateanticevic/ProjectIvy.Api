using ProjectIvy.Model.Binding.Country;
using ProjectIvy.Model.Binding.Trip;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using View = ProjectIvy.Model.View.Country;

namespace ProjectIvy.Business.Handlers.Country
{
    public interface ICountryHandler
    {
        long Count(CountryGetBinding binding);

        long CountVisited();

        View.Country Get(string id);

        PagedView<View.Country> Get(CountryGetBinding binding);

        IEnumerable<View.CountryBoundaries> GetBoundaries(IEnumerable<View.Country> countries);

        IEnumerable<View.Country> GetVisited(TripGetBinding binding);
    }
}
