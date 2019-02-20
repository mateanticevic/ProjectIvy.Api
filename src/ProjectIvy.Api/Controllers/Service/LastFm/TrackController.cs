using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Services.LastFm;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Constants.Database;
using ProjectIvy.Model.View.Services.LastFm;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectIvy.Api.Controllers.Service.LastFm
{
    [Authorize(Roles = UserRole.User)]
    [Route("Service/LastFm/[controller]")]
    public class TrackController : BaseController<TrackController>
    {
        private readonly ILastFmHandler _lastFmHandler;

        public TrackController(ILogger<TrackController> logger, ILastFmHandler lastFmHandler) : base(logger) => _lastFmHandler = lastFmHandler;

        [HttpGet]
        public async Task<IEnumerable<Track>> GetTracks([FromQuery] FilteredPagedBinding binding) => await _lastFmHandler.GetTracks(binding);

        [HttpGet("Count")]
        public async Task<int> GetCount() => await _lastFmHandler.GetTotalCount();

        [HttpGet("Loved")]
        public async Task<IEnumerable<Track>> GetLoved() => await _lastFmHandler.GetLovedTracks();

        [HttpGet("Top")]
        public async Task<IEnumerable<Track>> GetTopTracks() => await _lastFmHandler.GetTopTracks();
    }
}
