using AnticevicApi.Model.Binding.Poi;
using System.Collections.Generic;

namespace AnticevicApi.BL.Handlers.Poi
{
    public interface IPoiHandler : IHandler
    {
        void Create(PoiBinding binding);

        IEnumerable<Model.View.Poi.Poi> Get(PoiGetBinding binding);
    }
}
