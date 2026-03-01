using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Tag;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Tag;

namespace ProjectIvy.Business.Handlers.Tag;

public interface ITagHandler
{
    Task<View.Tag> Create(TagBinding binding);

    Task<PagedView<View.Tag>> Get(TagGetBinding binding);
}
