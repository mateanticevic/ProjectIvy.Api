using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Tracking;
using System.Threading.Tasks;

namespace ProjectIvy.Api.Controllers.Tracking
{
    [Route("Tracking")]
    [Authorize(ApiScopes.TrackingViewLast)]
    public class LastTrackingController : BaseController<TrackingController>
    {
        private readonly ITrackingHandler _trackingHandler;

        public LastTrackingController(ILogger<TrackingController> logger, ITrackingHandler trackingHandler) : base(logger)
        {
            _trackingHandler = trackingHandler;
        }

        [HttpGet("Last")]
        public async Task<IActionResult> GetLast([FromQuery] DateTime? at = null) => Ok(await _trackingHandler.GetLast(at));
    }
}
