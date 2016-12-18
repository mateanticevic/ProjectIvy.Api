using AnticevicApi.BL.Handlers.Movie;
using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.Constants;
using AnticevicApi.Model.View.Movie;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;

namespace AnticevicApi.Controllers
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
        public IEnumerable<Movie> Get([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int? page = null, [FromQuery] int? pageSize = null)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Get), from, to);

            return _movieHandler.Get(new FilteredPagedBinding(from, to, page, pageSize));
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
