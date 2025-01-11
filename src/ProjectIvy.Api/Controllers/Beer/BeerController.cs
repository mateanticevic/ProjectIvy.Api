using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Beer;
using ProjectIvy.Model.Binding.Beer;

namespace ProjectIvy.Api.Controllers.Beer;

[Authorize(ApiScopes.BeerUser)]
public class BeerController : BaseController<BeerController>
{
    private readonly IBeerHandler _beerHandler;

    public BeerController(ILogger<BeerController> logger, IBeerHandler beerHandler) : base(logger)
    {
        _beerHandler = beerHandler;
    }

    [HttpGet]
    [Authorize(ApiScopes.BeerUser)]
    public async Task<IActionResult> Get([FromQuery] BeerGetBinding binding) => Ok(await _beerHandler.GetBeers(binding));

    [HttpGet("{id}")]
    [Authorize(ApiScopes.BeerUser)]
    public async Task<IActionResult> Get(string id) => Ok(await _beerHandler.GetBeer(id));

    [HttpGet("Brand")]
    [Authorize(ApiScopes.BeerUser)]
    public async Task<IActionResult> GetBrands([FromQuery] BrandGetBinding binding) => Ok(await _beerHandler.GetBrands(binding));

    [HttpPost("Brand/{brandId}/Beer")]
    [Authorize(ApiScopes.BeerCreate)]
    public async Task<IActionResult> PostBeer(string brandId, [FromBody] BeerBinding binding) => Ok(await _beerHandler.CreateBeer(brandId, binding));

    [HttpPost("Brand")]
    [Authorize(ApiScopes.BeerCreate)]
    public async Task<IActionResult> PostBrand([FromBody] BrandBinding binding) => Ok(await _beerHandler.CreateBrand(binding));

    [HttpPut("{beerId}")]
    [Authorize(ApiScopes.BeerCreate)]
    public async Task PutBeer(string beerId, [FromBody] BeerBinding binding) => await _beerHandler.UpdateBeer(beerId, binding);

    [HttpPut("Brand/{brandId}")]
    [Authorize(ApiScopes.BeerCreate)]
    public async Task PutBrand(string brandId, [FromBody] BrandBinding binding) => await _beerHandler.UpdateBrand(brandId, binding);
}
