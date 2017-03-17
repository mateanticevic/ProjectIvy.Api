using AnticevicApi.BL.Handlers.Movie;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AnticevicApi.Model.Binding.Movie;

namespace AnticevicApi.Controllers.Movie
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
