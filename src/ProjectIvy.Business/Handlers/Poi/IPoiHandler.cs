using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Poi;
using ProjectIvy.Model.View;

namespace ProjectIvy.Business.Handlers.Poi;

public interface IPoiHandler : IHandler
{
    Task Create(PoiBinding binding);

    Task<PagedView<Model.View.Poi.Poi>> Get(PoiGetBinding binding);

    Task<IEnumerable<Model.View.Poi.PoiCategory>> GetCategories();
}
