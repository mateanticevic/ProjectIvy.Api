using ProjectIvy.Model.Binding.Consumation;

namespace ProjectIvy.BL.Handlers.Consumation
{
    public interface IConsumationHandler : IHandler
    {
        int Count(ConsumationGetBinding binding);

        int CountUniqueBeers(ConsumationGetBinding binding);

        int CountUniqueBrands(ConsumationGetBinding binding);

        int SumVolume(ConsumationGetBinding binding);
    }
}
