using AnticevicApi.BL.Services.LastFm;
using AnticevicApi.Common.Configuration;
using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.View.Services.LastFm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace AnticevicApi.Controllers.Service
{
    [Route("service/[controller]")]
    public class LastFmController : BaseController<LastFmController>
    {
        public LastFmController(IOptions<AppSettings> options, ILogger<LastFmController> logger, ILastFmHandler lastFmHandler) : base(options, logger)
        {
            LastFmHandler = lastFmHandler;
        }

        [HttpGet]
        [Route("count")]
        public async Task<int> GetCount()
        {
            return await LastFmHandler.GetTotalCount();
        }

        [HttpGet]
        [Route("tracks")]
        public async Task<IEnumerable<Track>> GetTracks([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int? page, [FromQuery] int? pageSize)
        {
            return await LastFmHandler.GetTracks(new FilteredPagedBinding(from, to, page, pageSize));
        }
    }
}
