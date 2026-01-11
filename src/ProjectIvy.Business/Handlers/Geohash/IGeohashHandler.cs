using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Geohash;

namespace ProjectIvy.Business.Handlers.Geohash;

public interface IGeohashHandler
{
    Task AddGeohashToCity(string cityValueId, IEnumerable<string> geohashes);

    Task AddGeohashToCountry(string countryValueId, IEnumerable<string> geohashes);

    Task AddGeohashToLocation(string locationValueId, IEnumerable<string> geohashes);

    Task<int> CountUnique(GeohashUniqueGetBinding binding);

    Task DeleteTrackings(string geohash);

    Task<IEnumerable<Model.View.Geohash.RouteTime>> FromGeohashToGeohash(IEnumerable<string> fromGeohashes, IEnumerable<string> toGeohashes, RouteTimeSort sort);

    Task<IEnumerable<DateOnly>> GetDays(string geohash);

    Task<IEnumerable<string>> GetChildren(string geohash, GeohashChildrenGetBinding b);

    Task<Model.View.City.City> GetCity(string geohash);

    Task<Model.View.Country.Country> GetCountry(string geohash);

    Task<Model.View.Geohash.Geohash> GetGeohash(string geohashId);

    Task<IEnumerable<string>> GetGeohashes(GeohashGetBinding binding);

    Task<IEnumerable<string>> GetCityGeohashes(string cityValueId);

    Task<IEnumerable<string>> GetCityGeohashesVisited(string cityValueId, GeohashCityVisitedGetBinding binding);

    Task<IEnumerable<string>> GetCountryGeohashes(string countryValueId);

    Task<IEnumerable<string>> GetCountryGeohashesVisited(string countryValueId, GeohashCountryVisitedGetBinding binding);

    Task<IEnumerable<string>> GetUnique(GeohashUniqueGetBinding binding);

    Task RemoveGeohashFromCity(string cityValueId, IEnumerable<string> geohashes);

    Task RemoveGeohashFromCountry(string cityValueId, IEnumerable<string> geohashes);

    Task RemoveGeohashFromLocation(string locationValueId, IEnumerable<string> geohashes);

    Task UpdateLocation(int locationId);
}
