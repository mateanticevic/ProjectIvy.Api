﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Services.LastFm;
using ProjectIvy.Model.Constants.Database;
using ProjectIvy.Model.View.Services.LastFm;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectIvy.Api.Controllers.Service.LastFm
{
    [Route("Service/LastFm/[controller]")]
    public class ArtistController : BaseController<ArtistController>
    {
        private readonly ILastFmHandler _lastFmHandler;

        public ArtistController(ILogger<ArtistController> logger, ILastFmHandler lastFmHandler) : base(logger) => _lastFmHandler = lastFmHandler;

        [HttpGet("Top")]
        public async Task<IEnumerable<Artist>> GetTopArtists() => await _lastFmHandler.GetTopArtists();
    }
}