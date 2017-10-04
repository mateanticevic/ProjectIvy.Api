using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.Movie;
using ProjectIvy.Model.Binding.Movie;
using ProjectIvy.Model.Constants.Database;

namespace ProjectIvy.Api.Controllers.Movie
{
    [Authorize(Roles = UserRole.User)]
    [Route("movie/rating")]
    public class MovieRatingController : BaseController<MovieController>
    {
        private readonly IMovieHandler _movieHandler;

        public MovieRatingController(ILogger<MovieController> logger, IMovieHandler movieHandler) : base(logger)
        {
            _movieHandler = movieHandler;
        }

        [HttpGet]
        [Route("average")]
        public double GetAverage([FromQuery] MovieGetBinding binding)
        {
            return _movieHandler.GetRatingAverage(binding);
        }
    }
}
