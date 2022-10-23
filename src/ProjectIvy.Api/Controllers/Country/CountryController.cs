using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Country;
using ProjectIvy.Model.Binding.Country;
using ProjectIvy.Model.Binding.Trip;
using ProjectIvy.Model.View;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.Country;

namespace ProjectIvy.Api.Controllers.Country
{
    public class CountryController : BaseController<CountryController>
    {
        private readonly ICountryHandler _countryHandler;

        public CountryController(ILogger<CountryController> logger, ICountryHandler countryHandler) : base(logger) => _countryHandler = countryHandler;

        [HttpGet]
        public PagedView<View.Country> Get(CountryGetBinding binding) => _countryHandler.Get(binding);

        [HttpGet("Count")]
        public long GetCount(CountryGetBinding binding) => _countryHandler.Count(binding);

        [HttpGet("{id}")]
        public View.Country Get(string id) => _countryHandler.Get(id);

        [HttpGet("Visited")]
        public IEnumerable<View.Country> GetVisited(TripGetBinding binding) => _countryHandler.GetVisited(binding);

        [HttpGet("Visited/Count")]
        public long GetVisitedCount() => _countryHandler.CountVisited();

        [HttpGet("Visited/Boundaries")]
        public IEnumerable<View.CountryBoundaries> GetVisitedBoundaries(TripGetBinding binding) => _countryHandler.GetBoundaries(_countryHandler.GetVisited(binding));

        [HttpGet("List")]
        public async Task<IActionResult> GetCountryLists() => Ok(await _countryHandler.GetLists());

        [HttpGet("List/Visited")]
        public async Task<IActionResult> GetCountryListsVisited() => Ok(await _countryHandler.GetListsVisited());
    }
}
