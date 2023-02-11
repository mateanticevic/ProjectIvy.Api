using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Geohash;

namespace ProjectIvy.Business.Handlers.Geohash
{
    public interface IGeohashHandler
    {
        Task<IEnumerable<string>> GetChildren(string geohash);

        Task<Model.View.City.City> GetCity(string geohash);

        Task<Model.View.Country.Country> GetCountry(string geohash);

        Task<Model.View.Geohash.Geohash> GetGeohash(string geohashId);

        Task<IEnumerable<string>> GetGeohashes(GeohashGetBinding binding);

        Task<IEnumerable<string>> GetCountryGeohashes(string countryValueId);
    }
}