using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.Movie;
using ProjectIvy.Model.Binding.Movie;
using ProjectIvy.Model.Constants.Database;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Movie;

namespace ProjectIvy.Api.Controllers.Movie
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
        public PagedView<View.Movie> Get([FromQuery] MovieGetBinding binding)
        {
            return _movieHandler.Get(binding);
        }

        [HttpGet("Count")]
        public int GetCount([FromQuery] MovieGetBinding binding)
        {
            return _movieHandler.GetCount(binding);
        }

        #endregion
    }
}
