using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.Beer;
using ProjectIvy.Model.Binding.Beer;

namespace ProjectIvy.Api.Controllers.Beer
{
    [Route("[controller]")]
    public class BeerController : BaseController<BeerController>
    {
        private readonly IBeerHandler _beerHandler;

        public BeerController(ILogger<BeerController> logger, IBeerHandler beerHandler) : base(logger)
        {
            _beerHandler = beerHandler;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] BeerGetBinding binding)
        {
            return Ok(_beerHandler.GetBeers(binding));
        }

        [HttpGet("Brand")]
        public IActionResult GetBrands() => Ok(_beerHandler.GetBrands());
    }
}
