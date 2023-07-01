using System.Threading.Tasks;

namespace ProjectIvy.Business.Handlers.Location
{
    public interface ILocationHandler
    {
        Task<IEnumerable<DateTime>> GetDays(string locationId);
    }
}