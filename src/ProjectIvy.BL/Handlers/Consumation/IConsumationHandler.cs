using ProjectIvy.Model.Binding.Consumation;

namespace ProjectIvy.BL.Handlers.Consumation
{
    public interface IConsumationHandler : IHandler
    {
        int Count(ConsumationGetBinding binding);

        int VolumeSum(ConsumationGetBinding binding);
    }
}
