using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Beer;
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
        public IActionResult Get([FromQuery] BeerGetBinding binding) => Ok(_beerHandler.GetBeers(binding));

        [HttpGet("{id}")]
        public IActionResult Get(string id) => Ok(_beerHandler.GetBeer(id));

        [HttpGet("Brand")]
        public IActionResult GetBrands() => Ok(_beerHandler.GetBrands());

        [HttpPost("Brand/{brandId}/Beer")]
        public IActionResult PostBeer(string brandId, [FromBody] BeerBinding binding) => Ok(_beerHandler.CreateBeer(brandId, binding));

        [HttpPost("Brand")]
        public IActionResult PostBrand([FromBody] string name) => Ok(_beerHandler.CreateBrand(name));
    }
}
