using System.Threading.Tasks;

namespace ProjectIvy.Business.Handlers.Location
{
    public interface ILocationHandler
    {
        Task<IEnumerable<Model.View.Location.Location>> Get();

        Task<IEnumerable<DateTime>> GetDays(string locationId);

        Task SetGeohashes(string locationValueId, IEnumerable<string> geohashes);
    }
}