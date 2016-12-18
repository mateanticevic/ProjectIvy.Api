using AnticevicApi.BL.Services.LastFm;
using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.View.Services.LastFm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace AnticevicApi.Controllers.Service
{
    [Route("service/[controller]")]
    public class LastFmController : BaseController<LastFmController>
    {
        private readonly ILastFmHandler _lastFmHandler;

        public LastFmController(ILogger<LastFmController> logger, ILastFmHandler lastFmHandler) : base(logger)
        {
            _lastFmHandler = lastFmHandler;
        }

        [HttpGet]
        [Route("count")]
        public async Task<int> GetCount()
        {
            return await _lastFmHandler.GetTotalCount();
        }

        [HttpGet]
        [Route("tracks")]
        public async Task<IEnumerable<Track>> GetTracks([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int? page, [FromQuery] int? pageSize)
        {
            return await _lastFmHandler.GetTracks(new FilteredPagedBinding(from, to, page, pageSize));
        }
    }
}
