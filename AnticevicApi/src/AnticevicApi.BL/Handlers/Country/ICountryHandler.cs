using AnticevicApi.Model.Binding.Country;
using AnticevicApi.Model.View;
using System.Collections.Generic;
using View = AnticevicApi.Model.View.Country;

namespace AnticevicApi.BL.Handlers.Country
{
    public interface ICountryHandler
    {
        long Count(CountryGetBinding binding);

        long CountVisited();

        View.Country Get(string id);

        PagedView<View.Country> Get(CountryGetBinding binding);

        IEnumerable<View.CountryBoundaries> GetBoundaries(IEnumerable<View.Country> countries);

        IEnumerable<View.Country> GetVisited();
    }
}
