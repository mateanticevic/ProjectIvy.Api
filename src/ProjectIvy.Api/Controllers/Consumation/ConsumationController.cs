using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Consumation;
using ProjectIvy.Business.Handlers.Country;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Consumation;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Consumation;

namespace ProjectIvy.Api.Controllers.Consumation;

[Authorize(ApiScopes.BeerUser)]
public class ConsumationController : BaseController<ConsumationController>
{
    private readonly IConsumationHandler _consumationHandler;

    private readonly ICountryHandler _countryHandler;

    public ConsumationController(ILogger<ConsumationController> logger,
                                 IConsumationHandler consumationHandler,
                                 ICountryHandler countryHandler) : base(logger)
    {
        _consumationHandler = consumationHandler;
        _countryHandler = countryHandler;
    }

    [HttpGet]
    public async Task<PagedView<View.Consumation>> Get(ConsumationGetBinding binding) => await _consumationHandler.Get(binding);

    [HttpGet("Alcohol/ByYear")]
    public async Task<IActionResult> GetAlcoholByYear(ConsumationGetBinding binding) => Ok(await _consumationHandler.AlcoholByYear(binding));

    [HttpGet("Average/ByYear")]
    public async Task<IActionResult> GetAverageByYear(ConsumationGetBinding binding) => Ok(await _consumationHandler.AverageByYear(binding));

    [HttpGet("Beer")]
    public async Task<IActionResult> GetBeer(FilteredPagedBinding binding) => Ok(await _consumationHandler.GetBeers(binding));

    [HttpGet("Count/Beer")]
    [HttpGet("Beer/Count")]
    public async Task<int> GetBeerCount(ConsumationGetBinding binding) => await _consumationHandler.CountBeers(binding);

    [HttpGet("Beer/New")]
    public async Task<IActionResult> GetBeerNew(ConsumationGetBinding binding) => Ok(await _consumationHandler.GetNewBeers(binding));

    [HttpGet("Count/Brand")]
    [HttpGet("Brand/Count")]
    public async Task<int> GetBrandCount(ConsumationGetBinding binding) => await _consumationHandler.CountBrands(binding);

    [HttpGet("Brand")]
    public async Task<IActionResult> GetBrands(FilteredPagedBinding binding) => Ok(await _consumationHandler.GetBrands(binding));

    [HttpGet("Consecutive/Days")]
    public async Task<IActionResult> GetConsecutiveDays(ConsumationGetBinding binding) => Ok(await _consumationHandler.ConsecutiveDates(binding));

    [HttpGet("Count")]
    public async Task<int> GetCount(ConsumationGetBinding binding) => await _consumationHandler.Count(binding);

    [HttpGet("Count/ByBeer")]
    public async Task<IActionResult> GetCountByBeer(ConsumationGetBinding binding) => Ok(await _consumationHandler.CountByBeer(binding));

    [HttpGet("Count/ByMonth")]
    public async Task<IEnumerable<KeyValuePair<string, int>>> GetCountByMonth([FromQuery] ConsumationGetBinding binding) => await _consumationHandler.CountByMonth(binding);

    [HttpGet("Count/ByMonthOfYear")]
    public async Task<IEnumerable<KeyValuePair<string, int>>> GetCountByMonthOfYear([FromQuery] ConsumationGetBinding binding) => await _consumationHandler.CountByMonthOfYear(binding);

    [HttpGet("Count/ByYear")]
    public async Task<IActionResult> GetCountByYear([FromQuery] ConsumationGetBinding binding) => Ok(await _consumationHandler.CountByYear(binding));

    [HttpGet("Country")]
    public async Task<IActionResult> GetCountries(ConsumationGetBinding binding) => Ok(await _consumationHandler.GetCountries(binding));

    [HttpGet("Country/Boundaries")]
    public async Task<IActionResult> GetCountryBoundaries(ConsumationGetBinding binding)
    {
        var countries = await _consumationHandler.GetCountries(binding);
        return Ok(await _countryHandler.GetBoundaries(countries));
    }

    [HttpGet("Sum")]
    public async Task<int> GetSum(ConsumationGetBinding binding) => await _consumationHandler.SumVolume(binding);

    [HttpGet("Sum/ByDay")]
    public async Task<IActionResult> GetSumByDay(ConsumationGetBinding binding) => Ok(await _consumationHandler.SumVolumeByDay(binding));

    [HttpGet("Sum/ByDayOfWeek")]
    public async Task<IActionResult> GetSumByDayOfWeek(ConsumationGetBinding binding) => Ok(await _consumationHandler.SumVolumeByDayOfWeek(binding));

    [HttpGet("Sum/ByMonth")]
    public async Task<IActionResult> GetSumByMonth(ConsumationGetBinding binding) => Ok(await _consumationHandler.SumVolumeByMonth(binding));

    [HttpGet("Sum/ByMonthOfYear")]
    public async Task<IActionResult> GetSumByMonthOfYear(ConsumationGetBinding binding) => Ok(await _consumationHandler.SumVolumeByMonthOfYear(binding));

    [HttpGet("Sum/ByServing")]
    public async Task<IActionResult> GetSumByServing(ConsumationGetBinding binding) => Ok(await _consumationHandler.SumVolumeByServing(binding));

    [HttpGet("Sum/ByYear")]
    public async Task<IActionResult> GetSumByYear(ConsumationGetBinding binding) => Ok(await _consumationHandler.SumVolumeByYear(binding));

    [HttpGet("Sum/ByBeer")]
    public async Task<IActionResult> GetSumVolumeByBeer(ConsumationGetBinding binding) => Ok(await _consumationHandler.SumVolumeByBeer(binding));

    [HttpGet("Sum/ByBrand")]
    public async Task<IActionResult> GetSumVolumeByBrand(ConsumationGetBinding binding) => Ok(await _consumationHandler.SumVolumeByBrand(binding));

    [HttpGet("Sum/ByCountry")]
    public async Task<IActionResult> GetSumVolumeByCountry(ConsumationGetBinding binding) => Ok(await _consumationHandler.SumVolumeByCountry(binding));

    [HttpGet("Sum/ByStyle")]
    public async Task<IActionResult> GetSumVolumeByStyle(ConsumationGetBinding binding) => Ok(await _consumationHandler.SumVolumeByStyle(binding));

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ConsumationBinding binding)
    {
        await _consumationHandler.Add(binding);
        return Ok();
    }
}
