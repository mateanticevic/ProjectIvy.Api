using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Tracking;
using ProjectIvy.Model.View;
using System.Threading.Tasks;
using System.Xml.Linq;
using View = ProjectIvy.Model.View.Tracking;

namespace ProjectIvy.Business.Handlers.Tracking
{
    public interface ITrackingHandler : IHandler
    {
        bool Create(TrackingBinding binding);

        Task Create(IEnumerable<TrackingBinding> binding);

        Task Delete(IEnumerable<long> timestamps);

        IEnumerable<View.Tracking> Get(TrackingGetBinding binding);

        int Count(FilteredBinding binding);

        IEnumerable<GroupedByMonth<int>> CountByMonth(FilteredBinding binding);

        IEnumerable<KeyValuePair<int, int>> CountByYear(FilteredBinding binding);

        int CountUnique(FilteredBinding binding);

        double GetAverageSpeed(FilteredBinding binding);

        Task<IEnumerable<string>> GetDays(TrackingGetBinding binding);

        int GetDistance(FilteredBinding binding);

        double GetMaxSpeed(FilteredBinding binding);

        View.Tracking GetLast(DateTime? at = null);

        Task<View.TrackingLocation> GetLastLocation();

        Task ImportFromGpx(XDocument xml);

        bool ImportFromKml(XDocument kml);
    }
}
