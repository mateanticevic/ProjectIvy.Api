using AnticevicApi.BL.Handlers.Movie;
using AnticevicApi.Config;
using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.View.Movie;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class MovieController : BaseController
    {
        public MovieController(IOptions<AppSettings> options, IMovieHandler movieHandler) : base(options)
        {
            MovieHandler = movieHandler;
        }

        #region Get

        [HttpGet]
        public IEnumerable<Movie> Get([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int? page = null, [FromQuery] int? pageSize = null)
        {
            return MovieHandler.Get(new FilteredPagedBinding(from, to, page, pageSize));
        }

        [HttpGet]
        [Route("count")]
        public int GetCount([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            return MovieHandler.GetCount(new FilteredBinding(from, to));
        }

        #endregion
    }
}
