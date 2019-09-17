using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Call;
using ProjectIvy.Model.View;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.Call;

namespace ProjectIvy.Business.Handlers.Call
{
    public interface ICallHandler
    {
        Task<string> Create(CallBinding binding);

        Task<PagedView<View.Call>> Get(FilteredPagedBinding binding);

        Task<bool> IsNumberBlacklisted(string number);
    }
}