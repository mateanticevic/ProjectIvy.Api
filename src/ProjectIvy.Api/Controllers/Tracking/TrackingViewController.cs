using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Tracking;
using ProjectIvy.Model.Binding.Tracking;

namespace ProjectIvy.Api.Controllers.Tracking;

[Authorize]
public class TrackingViewController : BaseController<TrackingViewController>
{
    private readonly ITrackingViewHandler _trackingViewHandler;

    public TrackingViewController(ILogger<TrackingViewController> logger, ITrackingViewHandler trackingViewHandler)
        : base(logger) => _trackingViewHandler = trackingViewHandler;

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _trackingViewHandler.Get());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id) => Ok(await _trackingViewHandler.GetSingle(id));

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TrackingViewBinding binding)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var view = await _trackingViewHandler.Create(binding);
        return CreatedAtAction(nameof(Get), new { id = view.Id }, view);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] TrackingViewBinding binding)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        await _trackingViewHandler.Update(id, binding);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _trackingViewHandler.Delete(id);
        return NoContent();
    }
}
