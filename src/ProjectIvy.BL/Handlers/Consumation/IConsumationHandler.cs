﻿using ProjectIvy.Model.Binding.Consumation;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Consumation;

namespace ProjectIvy.BL.Handlers.Consumation
{
    public interface IConsumationHandler : IHandler
    {
        int Count(ConsumationGetBinding binding);

        PagedView<CountBy<Model.View.Beer.Beer>> CountByBeer(ConsumationGetBinding binding);

        int CountUniqueBeers(ConsumationGetBinding binding);

        int CountUniqueBrands(ConsumationGetBinding binding);

        PagedView<View.Consumation> Get(ConsumationGetBinding binding);

        PagedView<SumBy<Model.View.Beer.Beer>> SumVolumeByBeer(ConsumationGetBinding binding);

        int SumVolume(ConsumationGetBinding binding);
    }
}