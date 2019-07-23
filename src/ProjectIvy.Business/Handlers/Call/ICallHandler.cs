using ProjectIvy.Model.Binding;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Call;

namespace ProjectIvy.Business.Handlers.Call
{
    public interface ICallHandler
    {
        PagedView<View.Call> Get(FilteredPagedBinding binding);
    }
}