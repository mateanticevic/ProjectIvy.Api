using ProjectIvy.BL.Handlers.Movie;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Model.Binding.Movie;

namespace ProjectIvy.Api.Controllers.Movie
{
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
