using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Tracking;
using ProjectIvy.Model.View.Tracking;

namespace ProjectIvy.Api.Controllers.Tracking;

[Route("Tracking")]
[Authorize(ApiScopes.TrackingViewLast)]
public class LastTrackingController : BaseController<TrackingController>
{
    private readonly ITrackingHandler _trackingHandler;

    public LastTrackingController(ILogger<TrackingController> logger, ITrackingHandler trackingHandler) : base(logger)
    {
        _trackingHandler = trackingHandler;
    }

    [HttpGet("LastLocation")]
    public async Task<TrackingLocation> GetLastLocation() => await _trackingHandler.GetLastLocation();
}
