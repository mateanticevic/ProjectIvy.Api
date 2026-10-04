using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Tracking;

namespace ProjectIvy.Api.Controllers.Tracking;

[AllowAnonymous]
public class TrackingViewController : BaseController<TrackingViewController>
{
    private readonly ITrackingViewHandler _trackingViewHandler;

    public TrackingViewController(ILogger<TrackingViewController> logger, ITrackingViewHandler trackingViewHandler)
        : base(logger) => _trackingViewHandler = trackingViewHandler;

    [HttpGet("/user/{username}/trackingview/{viewIdentifier}")]
    public async Task<IActionResult> Get(string username, string viewIdentifier)
        => Ok(await _trackingViewHandler.Get(username, viewIdentifier));
}
