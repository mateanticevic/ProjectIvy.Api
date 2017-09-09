using ProjectIvy.Model.Binding.Poi;
using System.Collections.Generic;

namespace ProjectIvy.BL.Handlers.Poi
{
    public interface IPoiHandler : IHandler
    {
        void Create(PoiBinding binding);

        IEnumerable<Model.View.Poi.Poi> Get(PoiGetBinding binding);
    }
}
