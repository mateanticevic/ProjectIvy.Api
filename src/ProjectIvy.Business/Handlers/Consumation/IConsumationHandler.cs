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

        PagedView<CountBy<View.Beer.Beer>> CountByBeer(ConsumationGetBinding binding);

        IEnumerable<KeyValuePair<string, int>> CountByMonth(ConsumationGetBinding binding);

        IEnumerable<KeyValuePair<string, int>> CountByMonthOfYear(ConsumationGetBinding binding);

        IEnumerable<KeyValuePair<string, int>> CountByYear(ConsumationGetBinding binding);

        int CountBeers(ConsumationGetBinding binding);

        int CountBrands(ConsumationGetBinding binding);

        PagedView<View.Consumation.Consumation> Get(ConsumationGetBinding binding);

        PagedView<View.Beer.Beer> GetBeers(FilteredPagedBinding binding);

        PagedView<View.Beer.BeerBrand> GetBrands(FilteredPagedBinding binding);

        PagedView<View.Beer.Beer> GetNewBeers(FilteredPagedBinding binding);

        PagedView<SumBy<View.Beer.Beer>> SumVolumeByBeer(ConsumationGetBinding binding);

        PagedView<SumBy<View.Country.Country>> SumVolumeByCountry(ConsumationGetBinding binding);

        PagedView<GroupedByMonth<int>> SumVolumeByMonth(ConsumationGetBinding binding);

        PagedView<SumBy<View.Beer.BeerServing>> SumVolumeByServing(ConsumationGetBinding binding);


        PagedView<SumBy<View.Beer.BeerStyle>> SumVolumeByStyle(ConsumationGetBinding binding);

        int SumVolume(ConsumationGetBinding binding);
    }
}
