using AnticevicApi.BL.Handlers.Tracking;
using AnticevicApi.Common.Configuration;
using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.Binding.Tracking;
using AnticevicApi.Model.View.Tracking;
using AnticevicApi.Utilities.Geo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Xml.Linq;
using System;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class TrackingController : BaseController<TrackingController>
    {
        public TrackingController(IOptions<AppSettings> options, ILogger<TrackingController> logger, ITrackingHandler trackingHandler) : base(options, logger)
        {
            TrackingHandler = trackingHandler;
        }

        #region Get

        [HttpGet]
        public IEnumerable<Tracking> Get([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            return TrackingHandler.Get(new FilteredBinding(from, to));
        }

        [HttpGet]
        [Route("gpx")]
        public string GetGpx([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            return TrackingHandler.Get(new FilteredBinding(from, to))
                                  .ToGpx()
                                  .ToString();
        }

        [HttpGet]
        [Route("last")]
        public TrackingCurrent GetLast()
        {
            return TrackingHandler.GetLast();
        }

        [HttpGet]
        [Route("count")]
        public int GetCount([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            return TrackingHandler.GetCount(new FilteredBinding(from, to));
        }

        [HttpGet]
        [Route("unique/count")]
        public int GetUniqueCount([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            return TrackingHandler.GetUniqueCount(new FilteredBinding(from, to));
        }

        [HttpGet]
        [Route("distance")]
        public int GetDistance([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            return TrackingHandler.GetDistance(new FilteredBinding(from, to));
        }

        #endregion

        #region Put

        [HttpPut]
        public bool Put([FromBody] TrackingBinding binding)
        {
            return TrackingHandler.Create(binding);
        }

        [HttpPut]
        [Route("kml")]
        public bool PutKml([FromBody] string kmlRaw)
        {
            var kml = XDocument.Parse(kmlRaw);

            return TrackingHandler.ImportFromKml(kml);
        }

        #endregion
    }
}
