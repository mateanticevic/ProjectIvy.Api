using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Services.LastFm;
using ProjectIvy.Model.Binding.Common;
using ProjectIvy.Model.Constants.Database;
using ProjectIvy.Model.View.Services.LastFm;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ProjectIvy.Api.Controllers.Service
{
    [Authorize(Roles = UserRole.User)]
    [Route("Service/[controller]")]
    public class LastFmController : BaseController<LastFmController>
    {
        private readonly ILastFmHandler _lastFmHandler;

        public LastFmController(ILogger<LastFmController> logger, ILastFmHandler lastFmHandler) : base(logger)
        {
            _lastFmHandler = lastFmHandler;
        }

        [HttpGet("Count")]
        public async Task<int> GetCount()
        {
            return await _lastFmHandler.GetTotalCount();
        }

        [HttpGet("Tracks")]
        public async Task<IEnumerable<Track>> GetTracks([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int? page, [FromQuery] int? pageSize)
        {
            return await _lastFmHandler.GetTracks(new FilteredPagedBinding(from, to, page, pageSize));
        }
    }
}
