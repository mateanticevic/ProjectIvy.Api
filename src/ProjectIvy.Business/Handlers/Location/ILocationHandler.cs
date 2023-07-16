using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Geohash;
using ProjectIvy.Model.View.Geohash;

namespace ProjectIvy.Business.Handlers.Location
{
    public interface ILocationHandler
    {
        Task<IEnumerable<Model.View.Location.Location>> Get();

        Task<IEnumerable<DateTime>> GetDays(string locationId);

        Task<IEnumerable<RouteTime>> FromLocationToLocation(string fromLocationValueId, string toLocationValueId, RouteTimeSort sort);

        Task SetGeohashes(string locationValueId, IEnumerable<string> geohashes);
    }
}