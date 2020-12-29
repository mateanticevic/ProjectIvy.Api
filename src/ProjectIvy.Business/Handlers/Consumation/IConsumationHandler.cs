using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Consumation;
using ProjectIvy.Model.View;
using System;
using System.Collections.Generic;
using View = ProjectIvy.Model.View;

namespace ProjectIvy.Business.Handlers.Consumation
{
    public interface IConsumationHandler : IHandler
    {
        void Add(ConsumationBinding binding);

        IEnumerable<(DateTime From, DateTime To)> ConsecutiveDates(ConsumationGetBinding binding);

        int Count(ConsumationGetBinding binding);

        PagedView<KeyValuePair<View.Beer.Beer, int>> CountByBeer(ConsumationGetBinding binding);

        IEnumerable<KeyValuePair<string, int>> CountByMonth(ConsumationGetBinding binding);

        IEnumerable<KeyValuePair<string, int>> CountByMonthOfYear(ConsumationGetBinding binding);

        IEnumerable<KeyValuePair<string, int>> CountByYear(ConsumationGetBinding binding);

        int CountBeers(ConsumationGetBinding binding);

        int CountBrands(ConsumationGetBinding binding);

        PagedView<View.Consumation.Consumation> Get(ConsumationGetBinding binding);

        PagedView<View.Beer.Beer> GetBeers(FilteredPagedBinding binding);

        PagedView<View.Beer.BeerBrand> GetBrands(FilteredPagedBinding binding);

        PagedView<View.Beer.Beer> GetNewBeers(FilteredPagedBinding binding);

        PagedView<KeyValuePair<View.Beer.Beer, int>> SumVolumeByBeer(ConsumationGetBinding binding);

        PagedView<KeyValuePair<View.Country.Country, int>> SumVolumeByCountry(ConsumationGetBinding binding);

        IEnumerable<KeyValuePair<int, int>> SumVolumeByDayOfWeek(ConsumationGetBinding binding);

        IEnumerable<KeyValuePair<int, int>> SumVolumeByMonth(ConsumationGetBinding binding);

        IEnumerable<GroupedByMonth<int>> SumVolumeByMonthOfYear(ConsumationGetBinding binding);

        IEnumerable<KeyValuePair<int, int>> SumVolumeByYear(ConsumationGetBinding binding);

        IEnumerable<KeyValuePair<View.Beer.BeerServing, int>> SumVolumeByServing(ConsumationGetBinding binding);

        PagedView<KeyValuePair<View.Beer.BeerStyle, int>> SumVolumeByStyle(ConsumationGetBinding binding);

        int SumVolume(ConsumationGetBinding binding);
    }
}
