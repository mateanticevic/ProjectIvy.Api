using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Tracking;
using View = ProjectIvy.Model.View.Tracking;

namespace ProjectIvy.Api.Controllers.Tracking;

[AllowAnonymous]
public class UserTrackingViewController : BaseController<UserTrackingViewController>
{
    private readonly IUserTrackingViewHandler _trackingViewHandler;

    public UserTrackingViewController(ILogger<UserTrackingViewController> logger, IUserTrackingViewHandler trackingViewHandler)
        : base(logger) => _trackingViewHandler = trackingViewHandler;

    [HttpGet("/user/{username}/trackingview/{viewIdentifier}")]
    public async Task<IEnumerable<View.Tracking>> Get(string username, string viewIdentifier)
        => await _trackingViewHandler.Get(username, viewIdentifier);
}
