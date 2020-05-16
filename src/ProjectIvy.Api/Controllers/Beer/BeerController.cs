using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Beer;
using ProjectIvy.Model.Binding.Beer;
using System.Threading.Tasks;

namespace ProjectIvy.Api.Controllers.Beer
{
    public class BeerController : BaseController<BeerController>
    {
        private readonly IBeerHandler _beerHandler;

        public BeerController(ILogger<BeerController> logger, IBeerHandler beerHandler) : base(logger)
        {
            _beerHandler = beerHandler;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] BeerGetBinding binding) => Ok(await _beerHandler.GetBeers(binding));

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id) => Ok(await _beerHandler.GetBeer(id));

        [HttpGet("Brand")]
        public async Task<IActionResult> GetBrands() => Ok(await _beerHandler.GetBrands());

        [HttpPost("Brand/{brandId}/Beer")]
        public async Task<IActionResult> PostBeer(string brandId, [FromBody] BeerBinding binding) => Ok(await _beerHandler.CreateBeer(brandId, binding));

        [HttpPost("Brand")]
        public async Task<IActionResult> PostBrand([FromBody] BrandBinding binding) => Ok(await _beerHandler.CreateBrand(binding));

        [HttpPut("{beerId}")]
        public async Task PutBeer(string beerId, [FromBody] BeerBinding binding) => await _beerHandler.UpdateBeer(beerId, binding);

        [HttpPut("Brand/{brandId}")]
        public async Task PutBrand(string brandId, [FromBody] BrandBinding binding) => await _beerHandler.UpdateBrand(brandId, binding);
    }
}
