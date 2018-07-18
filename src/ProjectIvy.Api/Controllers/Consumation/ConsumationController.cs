using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.Consumation;
using ProjectIvy.Model.Binding.Consumation;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Consumation;

namespace ProjectIvy.Api.Controllers.Consumation
{
    [Route("[controller]")]
    public class ConsumationController : BaseController<ConsumationController>
    {
        private readonly IConsumationHandler _consumationHandler;

        public ConsumationController(ILogger<ConsumationController> logger, IConsumationHandler consumationHandler) : base(logger)
        {
            _consumationHandler = consumationHandler;
        }

        [HttpGet("Consecutive/Days")]
        public IActionResult GetConsecutiveDays(ConsumationGetBinding binding) => Ok(_consumationHandler.ConsecutiveDates(binding));

        [HttpGet("Count")]
        public int Count(ConsumationGetBinding binding) => _consumationHandler.Count(binding);

        [HttpGet("Count/ByBeer")]
        public PagedView<CountBy<Model.View.Beer.Beer>> CountByBeer(ConsumationGetBinding binding) => _consumationHandler.CountByBeer(binding);

        [HttpGet("Count/Beers")]
        public int CountBeers(ConsumationGetBinding binding) => _consumationHandler.CountUniqueBeers(binding);

        [HttpGet("Count/Brands")]
        public int CountBrands(ConsumationGetBinding binding) => _consumationHandler.CountUniqueBrands(binding);

        [HttpGet("")]
        public PagedView<View.Consumation> Get(ConsumationGetBinding binding) => _consumationHandler.Get(binding);

        [HttpPost]
        public IActionResult Post([FromBody] ConsumationBinding binding)
        {
            _consumationHandler.Add(binding);
            return Ok();
        }

        [HttpGet("Sum/ByBeer")]
        public PagedView<SumBy<Model.View.Beer.Beer>> GetSumVolumeByBeer(ConsumationGetBinding binding) => _consumationHandler.SumVolumeByBeer(binding);

        [HttpGet("Sum/ByMonth")]
        public PagedView<GroupedByMonth<int>> GetSumVolumeByMonth(ConsumationGetBinding binding) => _consumationHandler.SumVolumeByMonth(binding);

        [HttpGet("Sum/ByServing")]
        public PagedView<SumBy<Model.View.Beer.BeerServing>> GetSumVolumeByServing(ConsumationGetBinding binding) => _consumationHandler.SumVolumeByServing(binding);

        [HttpGet("Sum")]
        public int SumVolume(ConsumationGetBinding binding) => _consumationHandler.SumVolume(binding);
    }
}