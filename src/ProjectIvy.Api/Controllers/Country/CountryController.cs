using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Country;
using ProjectIvy.Business.Handlers.Geohash;
using ProjectIvy.Common.Helpers;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Country;
using ProjectIvy.Model.Binding.Trip;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Country;

namespace ProjectIvy.Api.Controllers.Country;

[Authorize(ApiScopes.BasicUser)]
public class CountryController : BaseController<CountryController>
{
    private readonly ICountryHandler _countryHandler;
    private readonly IGeohashHandler _geohashHandler;

    public CountryController(ILogger<CountryController> logger, ICountryHandler countryHandler, IGeohashHandler geohashHandler) : base(logger)
    {
        _countryHandler = countryHandler;
        _geohashHandler = geohashHandler;
    }

    [HttpGet]
    public PagedView<View.Country> Get(CountryGetBinding binding) => _countryHandler.Get(binding);

    [HttpGet("{countryId}/City")]
    public async Task<PagedView<Model.View.City.City>> GetCities(string countryId, [FromQuery] FilteredPagedBinding binding) => await _countryHandler.GetCities(countryId, binding);

    [HttpGet("Count")]
    public long GetCount(CountryGetBinding binding) => _countryHandler.Count(binding);

    [HttpGet("{countryId}")]
    public View.Country Get(string countryId) => _countryHandler.Get(countryId);

    [HttpGet("{countryId}/Geohash")]
    public async Task<IActionResult> GetGeohashes(string countryId) => Ok(await _geohashHandler.GetCountryGeohashes(countryId));

    [HttpGet("Single")]
    public async Task<IActionResult> GetSingle(double latitude, double longitude) => Ok(await _geohashHandler.GetCountry(GeohashHelper.LocationToGeohash(latitude, longitude)));

    [HttpGet("Visited")]
    public async Task<IEnumerable<View.Country>> GetVisited(TripGetBinding binding) => await _countryHandler.GetVisited(binding);

    [HttpGet("Visited/ByYear")]
    public async Task<IEnumerable<KeyValuePair<int, IEnumerable<View.Country>>>> GetVisitedByYear() => await _countryHandler.GetVisitedByYear();

    [HttpGet("Visited/ByDay")]
    public async Task<IEnumerable<KeyValuePair<DateTime, IEnumerable<string>>>> GetVisitedByDay(FilteredBinding binding)
        => await _countryHandler.GetCountriesByDay(binding);

    [HttpGet("Visited/Days")]
    public async Task<IEnumerable<KeyValuePair<View.Country, int>>> GetVisitedDays() => await _countryHandler.GetDaysInCountry();

    [HttpGet("Visited/Count")]
    public long GetVisitedCount() => _countryHandler.CountVisited();

    [HttpGet("Visited/Count/ByYear")]
    public async Task<IEnumerable<KeyValuePair<int, int>>> GetVisitedCountByYear() => await _countryHandler.GetVisitedCountByYear();

    [HttpGet("Visited/Boundaries")]
    public async Task<IEnumerable<View.CountryBoundaries>> GetVisitedBoundaries(TripGetBinding binding) => _countryHandler.GetBoundaries(await _countryHandler.GetVisited(binding));

    [HttpGet("List")]
    public async Task<IActionResult> GetCountryLists() => Ok(await _countryHandler.GetLists());

    [HttpGet("List/Visited")]
    public async Task<IActionResult> GetCountryListsVisited() => Ok(await _countryHandler.GetListsVisited());

    [HttpDelete("{countryId}/Geohash")]
    public async Task DeleteCountryGeohash(string countryId, [FromQuery] IEnumerable<string> ids)
        => await _geohashHandler.RemoveGeohashFromCountry(countryId, ids);

    [HttpPost("{countryId}/Geohash")]
    public async Task PostCountryGeohash(string countryId, [FromBody] IEnumerable<string> geohashes)
        => await _geohashHandler.AddGeohashToCountry(countryId, geohashes);
}
