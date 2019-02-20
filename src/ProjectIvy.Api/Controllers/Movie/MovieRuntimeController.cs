using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Movie;
using ProjectIvy.Model.Binding.Movie;
using ProjectIvy.Model.Constants.Database;

namespace ProjectIvy.Api.Controllers.Movie
{
    [Authorize(Roles = UserRole.User)]
    [Route("Movie/Runtime")]
    public class MovieRuntimeController : BaseController<MovieController>
    {
        private readonly IMovieHandler _movieHandler;

        public MovieRuntimeController(ILogger<MovieController> logger, IMovieHandler movieHandler) : base(logger)
        {
            _movieHandler = movieHandler;
        }

        [HttpGet("Average")]
        public int GetAverage([FromQuery] MovieGetBinding binding)
        {
            return _movieHandler.GetRuntimeAverage(binding);
        }

        [HttpGet("Sum")]
        public int GetSum([FromQuery] MovieGetBinding binding)
        {
            return _movieHandler.GetSum(binding, x => x.Runtime);
        }
    }
}
