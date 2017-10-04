using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.Tracking;
using ProjectIvy.Model.Binding.Common;
using ProjectIvy.Model.Binding.Tracking;
using ProjectIvy.Model.Constants.Database;
using ProjectIvy.Utilities.Geo;
using System.Collections.Generic;
using System.Xml.Linq;
using System;
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
        public IEnumerable<View.Tracking> Get([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            return _trackingHandler.Get(new FilteredBinding(from, to));
        }

        [HttpGet]
        [Route("gpx")]
        public string GetGpx([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            return _trackingHandler.Get(new FilteredBinding(from, to))
                                   .ToGpx()
                                   .ToString();
        }

        [HttpGet]
        [Route("last")]
        public View.TrackingCurrent GetLast([FromQuery] DateTime? at = null)
        {
            return _trackingHandler.GetLast(at);
        }

        [HttpGet]
        [Route("count")]
        public int GetCount([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            return _trackingHandler.Count(new FilteredBinding(from, to));
        }

        [HttpGet]
        [Route("unique/count")]
        public int GetUniqueCount([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            return _trackingHandler.CountUnique(new FilteredBinding(from, to));
        }

        [HttpGet]
        [Route("distance")]
        public int GetDistance([FromQuery] FilteredBinding binding)
        {
            return _trackingHandler.GetDistance(binding);
        }

        [HttpGet]
        [Route("speed/average")]
        public double GetAverageSpeed([FromQuery] FilteredBinding binding)
        {
            return _trackingHandler.GetAverageSpeed(binding);
        }

        [HttpGet]
        [Route("speed/max")]
        public double GetMaxSpeed([FromQuery] FilteredBinding binding)
        {
            return _trackingHandler.GetMaxSpeed(binding);
        }

        #endregion

        #region Put

        [HttpPut]
        public bool Put([FromBody] TrackingBinding binding)
        {
            return _trackingHandler.Create(binding);
        }

        [HttpPut]
        [Route("kml")]
        public bool PutKml([FromBody] string kmlRaw)
        {
            var kml = XDocument.Parse(kmlRaw);

            return _trackingHandler.ImportFromKml(kml);
        }

        #endregion
    }
}
