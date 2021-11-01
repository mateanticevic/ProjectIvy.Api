using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Geohash;

namespace ProjectIvy.Business.Handlers.Geohash
{
    public interface IGeohashHandler
    {
        Task<Model.View.Geohash.Geohash> GetGeohash(string geohashId);

        Task<IEnumerable<string>> GetGeohashes(GeohashGetBinding binding);
    }
}