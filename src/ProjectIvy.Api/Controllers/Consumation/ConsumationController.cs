using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Consumation;
using ProjectIvy.Business.Handlers.Country;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Consumation;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.Consumation;

namespace ProjectIvy.Api.Controllers.Consumation
{
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

        [HttpGet("Alcohol/ByYear")]
        public async Task<IActionResult> GetAlcoholByYear(ConsumationGetBinding binding) => Ok(await _consumationHandler.AlcoholByYear(binding));

        [HttpGet("Average/ByYear")]
        public async Task<IActionResult> GetAverageByYear(ConsumationGetBinding binding) => Ok(await _consumationHandler.AverageByYear(binding));

        [HttpGet]
        public PagedView<View.Consumation> Get(ConsumationGetBinding binding) => _consumationHandler.Get(binding);

        [HttpGet("Beer")]
        public IActionResult GetBeer(FilteredPagedBinding binding) => Ok(_consumationHandler.GetBeers(binding));

        [HttpGet("Beer/New")]
        public IActionResult GetBeerNew(FilteredPagedBinding binding) => Ok(_consumationHandler.GetNewBeers(binding));

        [HttpGet("Brand")]
        public IActionResult GetBrands(FilteredPagedBinding binding) => Ok(_consumationHandler.GetBrands(binding));

        [HttpGet("Consecutive/Days")]
        public IActionResult GetConsecutiveDays(ConsumationGetBinding binding) => Ok(_consumationHandler.ConsecutiveDates(binding));

        [HttpGet("Count")]
        public int GetCount(ConsumationGetBinding binding) => _consumationHandler.Count(binding);

        [HttpGet("Count/ByBeer")]
        public IActionResult GetCountByBeer(ConsumationGetBinding binding) => Ok(_consumationHandler.CountByBeer(binding));

        [HttpGet("Count/ByMonth")]
        public IEnumerable<KeyValuePair<string, int>> GetCountByMonth([FromQuery] ConsumationGetBinding binding) => _consumationHandler.CountByMonth(binding);

        [HttpGet("Count/ByMonthOfYear")]
        public IEnumerable<KeyValuePair<string, int>> GetCountByMonthOfYear([FromQuery] ConsumationGetBinding binding) => _consumationHandler.CountByMonthOfYear(binding);

        [HttpGet("Count/ByYear")]
        public IActionResult GetCountByYear([FromQuery] ConsumationGetBinding binding) => Ok(_consumationHandler.CountByYear(binding));

        [HttpGet("Count/Beer")]
        [HttpGet("Beer/Count")]
        public int GetBeerCount(ConsumationGetBinding binding) => _consumationHandler.CountBeers(binding);

        [HttpGet("Count/Brand")]
        [HttpGet("Brand/Count")]
        public int GetBrandCount(ConsumationGetBinding binding) => _consumationHandler.CountBrands(binding);

        [HttpGet("Country")]
        public async Task<IActionResult> GetCountries(ConsumationGetBinding binding) => Ok(await _consumationHandler.GetCountries(binding));

        [HttpGet("Country/Boundaries")]
        public async Task<IActionResult> GetCountryBoundaries(ConsumationGetBinding binding)
        {
            var countries = await _consumationHandler.GetCountries(binding);
            return Ok(_countryHandler.GetBoundaries(countries));
        }

        [HttpGet("Sum")]
        public int GetSum(ConsumationGetBinding binding) => _consumationHandler.SumVolume(binding);

        [HttpGet("Sum/ByBeer")]
        public IActionResult GetSumVolumeByBeer(ConsumationGetBinding binding) => Ok(_consumationHandler.SumVolumeByBeer(binding));

        [HttpGet("Sum/ByBrand")]
        public IActionResult GetSumVolumeByBrand(ConsumationGetBinding binding) => Ok(_consumationHandler.SumVolumeByBrand(binding));

        [HttpGet("Sum/ByCountry")]
        public IActionResult GetSumVolumeByCountry(ConsumationGetBinding binding) => Ok(_consumationHandler.SumVolumeByCountry(binding));

        [HttpGet("Sum/ByDayOfWeek")]
        public IActionResult GetSumByDayOfWeek(ConsumationGetBinding binding) => Ok(_consumationHandler.SumVolumeByDayOfWeek(binding));

        [HttpGet("Sum/ByMonth")]
        public IActionResult GetSumByMonth(ConsumationGetBinding binding) => Ok(_consumationHandler.SumVolumeByMonth(binding));

        [HttpGet("Sum/ByMonthOfYear")]
        public IActionResult GetSumByMonthOfYear(ConsumationGetBinding binding) => Ok(_consumationHandler.SumVolumeByMonthOfYear(binding));

        [HttpGet("Sum/ByYear")]
        public IActionResult GetSumByYear(ConsumationGetBinding binding) => Ok(_consumationHandler.SumVolumeByYear(binding));

        [HttpGet("Sum/ByServing")]
        public IActionResult GetSumByServing(ConsumationGetBinding binding) => Ok(_consumationHandler.SumVolumeByServing(binding));

        [HttpGet("Sum/ByStyle")]
        public IActionResult GetSumVolumeByStyle(ConsumationGetBinding binding) => Ok(_consumationHandler.SumVolumeByStyle(binding));

        [HttpPost]
        public IActionResult Post([FromBody] ConsumationBinding binding)
        {
            _consumationHandler.Add(binding);
            return Ok();
        }
    }
}