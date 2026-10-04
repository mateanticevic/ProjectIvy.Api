using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Tracking;
using View = ProjectIvy.Model.View.Tracking;

namespace ProjectIvy.Business.Handlers.Tracking;

public interface ITrackingViewHandler : IHandler
{
    Task<IEnumerable<View.TrackingView>> Get();
    Task<View.TrackingView> GetSingle(string id);
    Task<View.TrackingView> Create(TrackingViewBinding binding);
    Task Update(string id, TrackingViewBinding binding);
    Task Delete(string id);
}
