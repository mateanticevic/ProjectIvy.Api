using System.Threading.Tasks;
using View = ProjectIvy.Model.View.Tracking;

namespace ProjectIvy.Business.Handlers.Tracking;

public interface ITrackingViewHandler : IHandler
{
    Task<IEnumerable<View.Tracking>> Get(string username, string viewIdentifier);
}
