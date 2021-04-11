using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Ride;
using ProjectIvy.Model.Binding.Ride;
using ProjectIvy.Model.Constants.Database;

namespace ProjectIvy.Api.Controllers.Flight
{
    [Authorize(Roles = UserRole.User)]
    public class RideController : BaseController<RideController>
    {
        private readonly IRideHandler _rideHandler;

        public RideController(ILogger<RideController> logger, IRideHandler rideHandler) : base(logger)
        {
            _rideHandler = rideHandler;
        }

        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await _rideHandler.GetRides());

        [HttpPost]
        public async Task Post(RideBinding binding) => await _rideHandler.Create(binding);
    }
}