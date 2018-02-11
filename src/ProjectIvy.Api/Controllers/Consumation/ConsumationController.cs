using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.Consumation;
using ProjectIvy.Model.Binding.Consumation;
using ProjectIvy.Model.View;

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

        [HttpGet("Count")]
        public int Count(ConsumationGetBinding binding) => _consumationHandler.Count(binding);

        [HttpGet("Count/UniqueBeers")]
        public int CountUniqueBeers(ConsumationGetBinding binding) => _consumationHandler.CountUniqueBeers(binding);

        [HttpGet("Count/UniqueBrands")]
        public int CountUniqueBrands(ConsumationGetBinding binding) => _consumationHandler.CountUniqueBrands(binding);

        [HttpGet("Sum/ByBeer")]
        public PagedView<SumBy<Model.View.Beer.Beer>> SumVolumeByBeer(ConsumationGetBinding binding) => _consumationHandler.SumVolumeByBeer(binding);

        [HttpGet("Sum")]
        public int SumVolume(ConsumationGetBinding binding) => _consumationHandler.SumVolume(binding);
    }
}