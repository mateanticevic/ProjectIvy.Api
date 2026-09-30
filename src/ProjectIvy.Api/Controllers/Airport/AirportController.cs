using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Airport;
using ProjectIvy.Model.Binding.Airport;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Airport;

namespace ProjectIvy.Api.Controllers.Airport;

[Authorize(ApiScopes.FlightUser)]
public class AirportController : BaseController<AirportController>
{
    private readonly IAirportHandler _airportHandler;

    public AirportController(ILogger<AirportController> logger, IAirportHandler airportHandler) : base(logger)
    {
        _airportHandler = airportHandler;
    }

    [HttpGet]
    public async Task<PagedView<View.Airport>> Get([FromQuery] AirportGetBinding binding) => await _airportHandler.Get(binding);

    [HttpGet("Count")]
    public async Task<long> GetCount([FromQuery] AirportGetBinding binding) => await _airportHandler.Count(binding);
}
