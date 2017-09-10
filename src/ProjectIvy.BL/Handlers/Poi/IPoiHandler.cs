using ProjectIvy.Model.Binding.Poi;
using ProjectIvy.Model.View;

namespace ProjectIvy.BL.Handlers.Poi
{
    public interface IPoiHandler : IHandler
    {
        void Create(PoiBinding binding);

        PagedView<Model.View.Poi.Poi> Get(PoiGetBinding binding);
    }
}
