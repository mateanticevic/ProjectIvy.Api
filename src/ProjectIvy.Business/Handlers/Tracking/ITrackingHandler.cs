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
    Task<int> Count(FilteredBinding binding);

    Task<IEnumerable<GroupedByMonth<int>>> CountByMonth(FilteredBinding binding);

    Task<IEnumerable<KeyValuePair<int, int>>> CountByYear(FilteredBinding binding);

    Task<int> CountUnique(FilteredBinding binding);

    Task Create(TrackingBinding binding);

    Task Create(IEnumerable<TrackingBinding> binding);

    Task Delete(IEnumerable<long> timestamps);

    Task<IEnumerable<View.Tracking>> Get(TrackingGetBinding binding);

    Task<double> GetAverageSpeed(FilteredBinding binding);

    Task<IEnumerable<string>> GetDays(TrackingGetBinding binding);

    Task<IEnumerable<DateTime>> GetDaysAtLast(DateTime? at = null);

    Task<TrackingDetails> GetDetails(FilteredBinding binding);

    Task<int> GetDistance(FilteredBinding binding);

    Task<View.Tracking> GetLast(DateTime? at = null);

    Task<View.TrackingLocation> GetLastLocation();

    Task<double> GetMaxSpeed(FilteredBinding binding);

    Task ImportFromGpx(XDocument xml);

    Task<bool> ImportFromKml(XDocument kml);
}
