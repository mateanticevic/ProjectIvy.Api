using AnticevicApi.Model.Binding.Country;
using AnticevicApi.Model.View;
using View = AnticevicApi.Model.View.Country;

namespace AnticevicApi.BL.Handlers.Country
{
    public interface ICountryHandler
    {
        View.Country Get(string id);

        PagedView<View.Country> Get(CountryGetBinding binding);
    }
}
