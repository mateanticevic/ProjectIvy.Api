using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Consumation;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Consumation;
using ProjectIvy.Model.Constants.Database;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Consumation;

namespace ProjectIvy.Api.Controllers.Consumation
{
    [Authorize(Roles = UserRole.User)]
    [Route("[controller]")]
    public class ConsumationController : BaseController<ConsumationController>
    {
        private readonly IConsumationHandler _consumationHandler;

        public ConsumationController(ILogger<ConsumationController> logger, IConsumationHandler consumationHandler) : base(logger)
        {
            _consumationHandler = consumationHandler;
        }

        [HttpGet]
        public PagedView<View.Consumation> Get(ConsumationGetBinding binding) => _consumationHandler.Get(binding);

        [HttpGet("Beer")]
        public IActionResult GetBeer(FilteredPagedBinding binding) => Ok(_consumationHandler.GetBeers(binding));

        [HttpGet("Brand")]
        public IActionResult GetBrands(FilteredPagedBinding binding) => Ok(_consumationHandler.GetBrands(binding));

        [HttpGet("Consecutive/Days")]
        public IActionResult GetConsecutiveDays(ConsumationGetBinding binding) => Ok(_consumationHandler.ConsecutiveDates(binding));

        [HttpGet("Count")]
        public int GetCount(ConsumationGetBinding binding) => _consumationHandler.Count(binding);

        [HttpGet("Count/ByBeer")]
        public PagedView<CountBy<Model.View.Beer.Beer>> GetCountByBeer(ConsumationGetBinding binding) => _consumationHandler.CountByBeer(binding);

        [HttpGet("Count/Beer")]
        [HttpGet("Beer/Count")]
        public int GetBeerCount(ConsumationGetBinding binding) => _consumationHandler.CountBeers(binding);

        [HttpGet("Count/Brand")]
        [HttpGet("Brand/Count")]
        public int GetCountBrand(ConsumationGetBinding binding) => _consumationHandler.CountBrands(binding);

        [HttpGet("Sum")]
        public int GetSum(ConsumationGetBinding binding) => _consumationHandler.SumVolume(binding);

        [HttpGet("Sum/ByBeer")]
        public PagedView<SumBy<Model.View.Beer.Beer>> GetSumVolumeByBeer(ConsumationGetBinding binding) => _consumationHandler.SumVolumeByBeer(binding);

        [HttpGet("Sum/ByMonth")]
        public PagedView<GroupedByMonth<int>> GetSumVolumeByMonth(ConsumationGetBinding binding) => _consumationHandler.SumVolumeByMonth(binding);

        [HttpGet("Sum/ByServing")]
        public PagedView<SumBy<Model.View.Beer.BeerServing>> GetSumVolumeByServing(ConsumationGetBinding binding) => _consumationHandler.SumVolumeByServing(binding);

        [HttpPost]
        public IActionResult Post([FromBody] ConsumationBinding binding)
        {
            _consumationHandler.Add(binding);
            return Ok();
        }
    }
}