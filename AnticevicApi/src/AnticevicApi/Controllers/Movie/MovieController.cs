using AnticevicApi.BL.Handlers.Movie;
using AnticevicApi.Model.Binding.Movie;
using AnticevicApi.Model.Constants.Database;
using AnticevicApi.Model.Constants;
using AnticevicApi.Model.View;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using View = AnticevicApi.Model.View.Movie;

namespace AnticevicApi.Controllers.Movie
{
    [Authorize(Roles = UserRole.User)]
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
        public PaginatedView<View.Movie> Get([FromQuery] MovieGetBinding binding)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Get), binding);

            return _movieHandler.Get(binding);
        }

        [HttpGet]
        [Route("count")]
        public int GetCount([FromQuery] MovieGetBinding binding)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetCount), binding);

            return _movieHandler.GetCount(binding);
        }

        #endregion
    }
}
