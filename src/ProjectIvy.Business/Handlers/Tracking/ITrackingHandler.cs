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
        int Count(FilteredBinding binding);

        IEnumerable<GroupedByMonth<int>> CountByMonth(FilteredBinding binding);

        IEnumerable<KeyValuePair<int, int>> CountByYear(FilteredBinding binding);

        bool Create(TrackingBinding binding);

        Task Create(IEnumerable<TrackingBinding> binding);

        Task Delete(IEnumerable<long> timestamps);

        IEnumerable<View.Tracking> Get(TrackingGetBinding binding);

        Task<View.Tracking> GetLast(DateTime? at = null);

        Task<View.TrackingLocation> GetLastLocation();

        int CountUnique(FilteredBinding binding);

        double GetAverageSpeed(FilteredBinding binding);

        Task<IEnumerable<string>> GetDays(TrackingGetBinding binding);

        int GetDistance(FilteredBinding binding);

        double GetMaxSpeed(FilteredBinding binding);

        Task ImportFromGpx(XDocument xml);

        bool ImportFromKml(XDocument kml);
    }
}
