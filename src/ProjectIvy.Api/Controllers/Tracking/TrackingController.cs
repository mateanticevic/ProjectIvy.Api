using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Tracking;
using ProjectIvy.Common.Interfaces;
using ProjectIvy.Common.Parsers;
using ProjectIvy.Model.Binding.Tracking;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.View.Tracking;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Tracking;

namespace ProjectIvy.Api.Controllers.Tracking;

[Authorize(ApiScopes.TrackingUser)]
public class TrackingController : BaseController<TrackingController>
{
    private readonly ITrackingHandler _trackingHandler;

    public TrackingController(ILogger<TrackingController> logger, ITrackingHandler trackingHandler) : base(logger)
    {
        _trackingHandler = trackingHandler;
    }

    [HttpDelete("{timestamp}")]
    public async Task<IActionResult> Delete(long timestamp)
    {
        await _trackingHandler.Delete(new long[] { timestamp });
        return Ok();
    }

    [HttpGet]
    public async Task<IEnumerable<View.Tracking>> Get([FromQuery] TrackingGetBinding binding) => await _trackingHandler.Get(binding);

    [HttpGet("Speed/Average")]
    public async Task<double> GetAverageSpeed([FromQuery] FilteredBinding binding) => await _trackingHandler.GetAverageSpeed(binding);

    [HttpGet("Count")]
    public async Task<int> GetCount([FromQuery] FilteredBinding binding) => await _trackingHandler.Count(binding);

    [HttpGet("Count/ByMonth")]
    public async Task<IEnumerable<GroupedByMonth<int>>> GetCountByMonth([FromQuery] FilteredBinding binding) => await _trackingHandler.CountByMonth(binding);

    [HttpGet("Count/ByYear")]
    public async Task<IActionResult> GetCountByYear([FromQuery] FilteredBinding binding) => Ok(await _trackingHandler.CountByYear(binding));

    [HttpGet("Day")]
    public async Task<IActionResult> GetDays(TrackingGetBinding binding) => Ok(await _trackingHandler.GetDays(binding));

    [HttpGet("Details")]
    public async Task<TrackingDetails> GetDetails([FromQuery] FilteredBinding binding) => await _trackingHandler.GetDetails(binding);

    [HttpGet("Distance")]
    public async Task<int> GetDistance([FromQuery] FilteredBinding binding) => await _trackingHandler.GetDistance(binding);

    [HttpGet("Gpx")]
    public async Task<string> GetGpx([FromQuery] TrackingGetBinding binding)
    {
        return (await _trackingHandler.Get(binding))
                               .Select(x => (ITracking)x)
                               .ToGpx()
                               .ToString();
    }

    [HttpGet("Last")]
    public async Task<IActionResult> GetLast([FromQuery] DateTime? at = null) => Ok(await _trackingHandler.GetLast(at));

    [HttpGet("Last/Days")]
    public async Task<IEnumerable<DateTime>> GetLastDays([FromQuery] DateTime? at = null) => await _trackingHandler.GetDaysAtLast(at);

    [HttpGet("Speed/Max")]
    public async Task<double> GetMaxSpeed([FromQuery] FilteredBinding binding) => await _trackingHandler.GetMaxSpeed(binding);

    [HttpGet("Count/Unique")]
    public async Task<int> GetUniqueCount([FromQuery] FilteredBinding binding) => await _trackingHandler.CountUnique(binding);

    [HttpPost("Delete")]
    public async Task<IActionResult> PostDelete([FromBody] IEnumerable<long> timestamps)
    {
        await _trackingHandler.Delete(timestamps);
        return Ok();
    }

    [HttpPost("Gpx")]
    [Consumes("text/xml")]
    public async Task<IActionResult> PostGpx()
    {
        string xmlRaw;
        using (System.IO.StreamReader reader = new System.IO.StreamReader(Request.Body, Encoding.UTF8))
        {
            xmlRaw = await reader.ReadToEndAsync();
        }
        var xml = XDocument.Parse(xmlRaw);

        await _trackingHandler.ImportFromGpx(xml);
        return Ok();
    }

    [HttpPut("Kml")]
    public async Task<bool> PutKml([FromBody] string kmlRaw)
    {
        var kml = XDocument.Parse(kmlRaw);

        return await _trackingHandler.ImportFromKml(kml);
    }
}
