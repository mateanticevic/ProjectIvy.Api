using ProjectIvy.Model.Binding.Consumation;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using System;
using View = ProjectIvy.Model.View.Consumation;

namespace ProjectIvy.BL.Handlers.Consumation
{
    public interface IConsumationHandler : IHandler
    {
        void Add(ConsumationBinding binding);

        IEnumerable<(DateTime From, DateTime To)> ConsecutiveDates(ConsumationGetBinding binding);

        int Count(ConsumationGetBinding binding);

        PagedView<CountBy<Model.View.Beer.Beer>> CountByBeer(ConsumationGetBinding binding);

        int CountUniqueBeers(ConsumationGetBinding binding);

        int CountUniqueBrands(ConsumationGetBinding binding);

        PagedView<View.Consumation> Get(ConsumationGetBinding binding);

        PagedView<SumBy<Model.View.Beer.Beer>> SumVolumeByBeer(ConsumationGetBinding binding);

        PagedView<SumBy<Model.View.Beer.BeerServing>> SumVolumeByServing(ConsumationGetBinding binding);

        int SumVolume(ConsumationGetBinding binding);
    }
}
