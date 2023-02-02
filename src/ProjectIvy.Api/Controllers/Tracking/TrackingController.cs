using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Tracking;
using ProjectIvy.Common.Interfaces;
using ProjectIvy.Common.Parsers;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Tracking;
using ProjectIvy.Model.View;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using View = ProjectIvy.Model.View.Tracking;

namespace ProjectIvy.Api.Controllers.Tracking
{
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
        public IEnumerable<View.Tracking> Get([FromQuery] TrackingGetBinding binding) => _trackingHandler.Get(binding);

        [HttpGet("Gpx")]
        public string GetGpx([FromQuery] TrackingGetBinding binding)
        {
            return _trackingHandler.Get(binding)
                                   .Select(x => (ITracking)x)
                                   .ToGpx()
                                   .ToString();
        }

        [HttpGet("Count")]
        public int GetCount([FromQuery] FilteredBinding binding) => _trackingHandler.Count(binding);

        [HttpGet("Count/ByMonth")]
        public IEnumerable<GroupedByMonth<int>> GetCountByMonth([FromQuery] FilteredBinding binding) => _trackingHandler.CountByMonth(binding);

        [HttpGet("Count/ByYear")]
        public IActionResult GetCountByYear([FromQuery] FilteredBinding binding) => Ok(_trackingHandler.CountByYear(binding));

        [HttpGet("Count/Unique")]
        public int GetUniqueCount([FromQuery] FilteredBinding binding) => _trackingHandler.CountUnique(binding);

        [HttpGet("Day")]
        public async Task<IActionResult> GetDays(TrackingGetBinding binding) => Ok(await _trackingHandler.GetDays(binding));

        [HttpGet("Distance")]
        public int GetDistance([FromQuery] FilteredBinding binding) => _trackingHandler.GetDistance(binding);

        [HttpGet("Last")]
        public async Task<IActionResult> GetLast([FromQuery] DateTime? at = null) => Ok(await _trackingHandler.GetLast(at));

        [HttpGet("LastLocation")]
        public async Task<IActionResult> GetLastLocation() => Ok(await _trackingHandler.GetLastLocation());

        [HttpGet("Speed/Average")]
        public double GetAverageSpeed([FromQuery] FilteredBinding binding) => _trackingHandler.GetAverageSpeed(binding);

        [HttpGet("Speed/Max")]
        public double GetMaxSpeed([FromQuery] FilteredBinding binding) => _trackingHandler.GetMaxSpeed(binding);

        [HttpPut]
        public bool Put([FromBody] TrackingBinding binding) => _trackingHandler.Create(binding);

        [HttpPost]
        public async Task Post([FromBody] IEnumerable<TrackingBinding> binding) => await _trackingHandler.Create(binding);

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
        public bool PutKml([FromBody] string kmlRaw)
        {
            var kml = XDocument.Parse(kmlRaw);

            return _trackingHandler.ImportFromKml(kml);
        }
    }
}
