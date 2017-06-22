using AnticevicApi.BL.Handlers.Tracking;
using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.Binding.Tracking;
using AnticevicApi.Model.Constants.Database;
using AnticevicApi.Utilities.Geo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Xml.Linq;
using System;
using View = AnticevicApi.Model.View.Tracking;

namespace AnticevicApi.Controllers.Tracking
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
        public View.TrackingCurrent GetLast()
        {
            return _trackingHandler.GetLast();
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
