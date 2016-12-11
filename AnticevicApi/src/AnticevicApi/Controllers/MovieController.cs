using AnticevicApi.BL.Handlers.Movie;
using AnticevicApi.Config;
using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.Constants;
using AnticevicApi.Model.View.Movie;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class MovieController : BaseController<MovieController>
    {
        public MovieController(IOptions<AppSettings> options, ILogger<MovieController> logger, IMovieHandler movieHandler) : base(options, logger)
        {
            MovieHandler = movieHandler;
        }

        #region Get

        [HttpGet]
        public IEnumerable<Movie> Get([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int? page = null, [FromQuery] int? pageSize = null)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Get), from, to);

            return MovieHandler.Get(new FilteredPagedBinding(from, to, page, pageSize));
        }

        [HttpGet]
        [Route("count")]
        public int GetCount([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetCount), from, to);

            return MovieHandler.GetCount(new FilteredBinding(from, to));
        }

        #endregion
    }
}
