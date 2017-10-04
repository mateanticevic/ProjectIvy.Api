using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.Movie;
using ProjectIvy.Model.Binding.Movie;
using ProjectIvy.Model.Constants.Database;

namespace ProjectIvy.Api.Controllers.Movie
{
    [Authorize(Roles = UserRole.User)]
    [Route("movie/runtime")]
    public class MovieRuntimeController : BaseController<MovieController>
    {
        private readonly IMovieHandler _movieHandler;

        public MovieRuntimeController(ILogger<MovieController> logger, IMovieHandler movieHandler) : base(logger)
        {
            _movieHandler = movieHandler;
        }

        [HttpGet]
        [Route("average")]
        public int GetAverage([FromQuery] MovieGetBinding binding)
        {
            return _movieHandler.GetRuntimeAverage(binding);
        }

        [HttpGet]
        [Route("sum")]
        public int GetSum([FromQuery] MovieGetBinding binding)
        {
            return _movieHandler.GetSum(binding, x => x.Runtime);
        }
    }
}
