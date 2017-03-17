using AnticevicApi.BL.Handlers.Movie;
using AnticevicApi.Model.Binding.Movie;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AnticevicApi.Controllers.Movie
{
    [Route("movie/myRating")]
    public class MovieMyRatingController : BaseController<MovieController>
    {
        private readonly IMovieHandler _movieHandler;

        public MovieMyRatingController(ILogger<MovieController> logger, IMovieHandler movieHandler) : base(logger)
        {
            _movieHandler = movieHandler;
        }

        [HttpGet]
        [Route("average")]
        public double GetAverage([FromQuery] MovieGetBinding binding)
        {
            return _movieHandler.GetMyRatingAverage(binding);
        }
    }
}
