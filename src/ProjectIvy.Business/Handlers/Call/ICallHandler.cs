using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Call;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Call;

namespace ProjectIvy.Business.Handlers.Call;

public interface ICallHandler
{
    Task<string> Create(CallBinding binding);

    Task<PagedView<View.Call>> Get(CallGetBinding binding);

    Task<bool> IsNumberBlacklisted(string number);
}