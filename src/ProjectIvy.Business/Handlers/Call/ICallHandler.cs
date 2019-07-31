using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Call;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Call;

namespace ProjectIvy.Business.Handlers.Call
{
    public interface ICallHandler
    {
        string Create(CallBinding binding);

        PagedView<View.Call> Get(FilteredPagedBinding binding);
    }
}