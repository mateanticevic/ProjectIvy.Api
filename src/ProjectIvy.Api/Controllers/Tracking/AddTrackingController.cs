using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Tracking;
using ProjectIvy.Model.Binding.Tracking;

namespace ProjectIvy.Api.Controllers.Tracking;

[Route("Tracking")]
[Authorize(ApiScopes.TrackingCreate)]
public class AddTrackingController : BaseController<AddTrackingController>
{
    private readonly ITrackingHandler _trackingHandler;

    public AddTrackingController(ILogger<AddTrackingController> logger, ITrackingHandler trackingHandler) : base(logger)
    {
        _trackingHandler = trackingHandler;
    }

    [HttpPut]
    public async Task Put([FromBody] TrackingBinding binding) => await _trackingHandler.Create(binding);

    [HttpPost]
    public async Task Post([FromBody] IEnumerable<TrackingBinding> binding) => await _trackingHandler.Create(binding);
}
