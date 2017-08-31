using AnticevicApi.Model.Binding.Country;
using AnticevicApi.Model.View;
using System.Collections.Generic;
using View = AnticevicApi.Model.View.Country;

namespace AnticevicApi.BL.Handlers.Country
{
    public interface ICountryHandler
    {
        View.Country Get(string id);

        PagedView<View.Country> Get(CountryGetBinding binding);

        IEnumerable<View.Country> GetVisited();
    }
}
