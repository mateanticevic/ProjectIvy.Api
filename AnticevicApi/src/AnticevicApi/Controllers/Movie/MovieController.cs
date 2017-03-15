using AnticevicApi.BL.Handlers.Movie;
using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using View = AnticevicApi.Model.View.Movie;
using AnticevicApi.Model.Binding.Movie;

namespace AnticevicApi.Controllers.Movie
{
    [Route("[controller]")]
    public class MovieController : BaseController<MovieController>
    {
        private readonly IMovieHandler _movieHandler;

        public MovieController(ILogger<MovieController> logger, IMovieHandler movieHandler) : base(logger)
        {
            _movieHandler = movieHandler;
        }

        #region Get

        [HttpGet]
        public IEnumerable<View.Movie> Get([FromQuery] MovieGetBinding binding)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Get), binding);

            return _movieHandler.Get(binding);
        }

        [HttpGet]
        [Route("count")]
        public int GetCount([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetCount), from, to);

            return _movieHandler.GetCount(new FilteredBinding(from, to));
        }

        #endregion
    }
}
