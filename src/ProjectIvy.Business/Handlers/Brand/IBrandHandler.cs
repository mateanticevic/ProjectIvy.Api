using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Brand;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Brand;

namespace ProjectIvy.Business.Handlers.Brand;

public interface IBrandHandler : IHandler
{
    View.Brand Get(string id);

    Task<PagedView<View.Brand>> Get(BrandGetBinding binding);
}
