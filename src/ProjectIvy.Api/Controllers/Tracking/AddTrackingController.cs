using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Tracking;
using ProjectIvy.Model.Binding.Tracking;
using System.Threading.Tasks;

namespace ProjectIvy.Api.Controllers.Tracking
{
    [Route("Tracking")]
    [Authorize(Policies.TrackingCreator)]
    public class AddTrackingController : BaseController<TrackingController>
    {
        private readonly ITrackingHandler _trackingHandler;

        public AddTrackingController(ILogger<TrackingController> logger, ITrackingHandler trackingHandler) : base(logger)
        {
            _trackingHandler = trackingHandler;
        }

        [HttpPut]
        public bool Put([FromBody] TrackingBinding binding) => _trackingHandler.Create(binding);

        [HttpPost]
        public async Task Post([FromBody] IEnumerable<TrackingBinding> binding) => await _trackingHandler.Create(binding);
    }
}
