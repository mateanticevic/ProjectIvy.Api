using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.Tracking;
using ProjectIvy.Common.Parsers;
using ProjectIvy.Model.Binding.Common;
using ProjectIvy.Model.Binding.Tracking;
using ProjectIvy.Model.Constants.Database;
using System.Collections.Generic;
using System.Xml.Linq;
using System;
using System.Linq;
using ProjectIvy.Common.Interfaces;
using View = ProjectIvy.Model.View.Tracking;

namespace ProjectIvy.Api.Controllers.Tracking
{
    [Authorize(Roles = UserRole.User)]
    [Route("[controller]")]
    public class TrackingController : BaseController<TrackingController>
    {
        private readonly ITrackingHandler _trackingHandler;

        public TrackingController(ILogger<TrackingController> logger, ITrackingHandler trackingHandler) : base(logger)
        {
            _trackingHandler = trackingHandler;
        }

        #region Get

        [HttpGet]
        public IEnumerable<View.Tracking> Get([FromQuery] FilteredBinding binding) => _trackingHandler.Get(binding);

        [HttpGet("Gpx")]
        public string GetGpx([FromQuery] FilteredBinding binding)
        {
            return _trackingHandler.Get(binding)
                                   .Select(x => (ITracking)x)
                                   .ToGpx()
                                   .ToString();
        }

        [HttpGet("Last")]
        public View.TrackingCurrent GetLast([FromQuery] DateTime? at = null) => _trackingHandler.GetLast(at);

        [HttpGet("Count")]
        public int GetCount([FromQuery] FilteredBinding binding) => _trackingHandler.Count(binding);

        [HttpGet("Count/Unique")]
        public int GetUniqueCount([FromQuery] FilteredBinding binding) => _trackingHandler.CountUnique(binding);

        [HttpGet("Distance")]
        public int GetDistance([FromQuery] FilteredBinding binding) => _trackingHandler.GetDistance(binding);

        [HttpGet("Speed/Average")]
        public double GetAverageSpeed([FromQuery] FilteredBinding binding) => _trackingHandler.GetAverageSpeed(binding);

        [HttpGet("Speed/Max")]
        public double GetMaxSpeed([FromQuery] FilteredBinding binding) => _trackingHandler.GetMaxSpeed(binding);

        #endregion

        #region Put

        [HttpPut]
        public bool Put([FromBody] TrackingBinding binding) => _trackingHandler.Create(binding);

        [HttpPut("Kml")]
        public bool PutKml([FromBody] string kmlRaw)
        {
            var kml = XDocument.Parse(kmlRaw);

            return _trackingHandler.ImportFromKml(kml);
        }

        #endregion
    }
}
