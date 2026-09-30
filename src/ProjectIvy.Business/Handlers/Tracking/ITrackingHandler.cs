using System.Threading.Tasks;
using System.Xml.Linq;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Tracking;
using ProjectIvy.Model.View;
using ProjectIvy.Model.View.Tracking;
using View = ProjectIvy.Model.View.Tracking;

namespace ProjectIvy.Business.Handlers.Tracking;

public interface ITrackingHandler : IHandler
{
    int Count(FilteredBinding binding);

    IEnumerable<GroupedByMonth<int>> CountByMonth(FilteredBinding binding);

    IEnumerable<KeyValuePair<int, int>> CountByYear(FilteredBinding binding);

    int CountUnique(FilteredBinding binding);

    Task Create(TrackingBinding binding);

    Task Create(IEnumerable<TrackingBinding> binding);

    Task Delete(IEnumerable<long> timestamps);

    IEnumerable<View.Tracking> Get(TrackingGetBinding binding);

    double GetAverageSpeed(FilteredBinding binding);

    Task<IEnumerable<string>> GetDays(TrackingGetBinding binding);

    Task<IEnumerable<DateTime>> GetDaysAtLast(DateTime? at = null);

    Task<TrackingDetails> GetDetails(FilteredBinding binding);

    int GetDistance(FilteredBinding binding);

    Task<View.Tracking> GetLast(DateTime? at = null);

    Task<View.TrackingLocation> GetLastLocation();

    double GetMaxSpeed(FilteredBinding binding);

    Task ImportFromGpx(XDocument xml);

    bool ImportFromKml(XDocument kml);
}
