using System.Threading.Tasks;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Common;
using ProjectIvy.Model.Binding.Geohash;
using ProjectIvy.Model.Binding.Location;
using ProjectIvy.Model.View;
using ProjectIvy.Model.View.Geohash;
using ProjectIvy.Model.View.Location;

namespace ProjectIvy.Business.Handlers.Location
{
    public interface ILocationHandler
    {
        Task Create(LocationBinding b);

        Task<PagedView<Model.View.Location.Location>> Get(LocationGetBinding b);

        Task<IEnumerable<KeyValuePair<DateTime, IEnumerable<Model.View.Location.Location>>>> GetByDay(FilteredBinding b);

        Task<IEnumerable<DateTime>> GetDays(string locationId);

        Task<IEnumerable<string>> GetGeohashes(string valueId);

         Task<IEnumerable<LocationType>> GetLocationTypes();

        Task<IEnumerable<RouteTime>> FromLocationToLocation(string fromLocationValueId, string toLocationValueId, RouteTimeSort sort);

        Task SetGeohashes(string locationValueId, IEnumerable<string> geohashes);
    }
}