using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Geohash;

namespace ProjectIvy.Business.Handlers.Geohash
{
    public interface IGeohashHandler
    {
        Task<IEnumerable<string>> GetGeohashes(GeohashGetBinding binding);
    }
}