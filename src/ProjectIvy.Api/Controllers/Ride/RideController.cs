using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Ride;
using ProjectIvy.Model.Binding.Ride;

namespace ProjectIvy.Api.Controllers.Ride;

public class RideController : BaseController<RideController>
{
    private readonly IRideHandler _rideHandler;

    public RideController(ILogger<RideController> logger, IRideHandler rideHandler) : base(logger)
    {
        _rideHandler = rideHandler;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] RideGetBinding binding) => Ok(await _rideHandler.GetRides(binding));

    [HttpPost]
    public async Task Post([FromBody] RideBinding binding) => await _rideHandler.Create(binding);
}
