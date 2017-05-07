using AnticevicApi.Model.Binding.Poi;

namespace AnticevicApi.BL.Handlers.Poi
{
    public interface IPoiHandler : IHandler
    {
        void Create(PoiBinding binding);
    }
}
