using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Consumation;
using ProjectIvy.Model.View;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View;

namespace ProjectIvy.Business.Handlers.Consumation;

public interface IConsumationHandler : IHandler
{
    Task Add(ConsumationBinding binding);

    Task<IEnumerable<KeyValuePair<int, decimal>>> AlcoholByYear(ConsumationGetBinding binding);

    Task<IEnumerable<KeyValuePair<int, int>>> AverageByYear(ConsumationGetBinding binding);

    Task<IEnumerable<(DateTime From, DateTime To)>> ConsecutiveDates(ConsumationGetBinding binding);

    Task<int> Count(ConsumationGetBinding binding);

    Task<int> CountBeers(ConsumationGetBinding binding);

    Task<int> CountBrands(ConsumationGetBinding binding);

    Task<PagedView<KeyValuePair<View.Beer.Beer, int>>> CountByBeer(ConsumationGetBinding binding);

    Task<IEnumerable<KeyValuePair<string, int>>> CountByMonth(ConsumationGetBinding binding);

    Task<IEnumerable<KeyValuePair<string, int>>> CountByMonthOfYear(ConsumationGetBinding binding);

    Task<IEnumerable<KeyValuePair<int, int>>> CountByYear(ConsumationGetBinding binding);

    Task<PagedView<View.Consumation.Consumation>> Get(ConsumationGetBinding binding);

    Task<PagedView<View.Beer.Beer>> GetBeers(FilteredPagedBinding binding);

    Task<PagedView<View.Beer.BeerBrand>> GetBrands(FilteredPagedBinding binding);

    Task<IEnumerable<View.Country.Country>> GetCountries(ConsumationGetBinding binding);

    Task<PagedView<View.Beer.Beer>> GetNewBeers(ConsumationGetBinding binding);

    Task<int> SumVolume(ConsumationGetBinding binding);

    Task<PagedView<KeyValuePair<View.Beer.Beer, int>>> SumVolumeByBeer(ConsumationGetBinding binding);

    Task<PagedView<KeyValuePair<View.Beer.BeerBrand, int>>> SumVolumeByBrand(ConsumationGetBinding binding);

    Task<PagedView<KeyValuePair<View.Country.Country, int>>> SumVolumeByCountry(ConsumationGetBinding binding);

    Task<IEnumerable<KeyValuePair<DateTime, int>>> SumVolumeByDay(ConsumationGetBinding binding);

    Task<IEnumerable<KeyValuePair<int, int>>> SumVolumeByDayOfWeek(ConsumationGetBinding binding);

    Task<IEnumerable<KeyValuePair<int, int>>> SumVolumeByMonth(ConsumationGetBinding binding);

    Task<IEnumerable<KeyValuePair<DateTime, int>>> SumVolumeByMonthOfYear(ConsumationGetBinding binding);

    Task<IEnumerable<KeyValuePair<View.Beer.BeerServing, int>>> SumVolumeByServing(ConsumationGetBinding binding);

    Task<PagedView<KeyValuePair<View.Beer.BeerStyle, int>>> SumVolumeByStyle(ConsumationGetBinding binding);

    Task<IEnumerable<KeyValuePair<int, int>>> SumVolumeByYear(ConsumationGetBinding binding);
}
