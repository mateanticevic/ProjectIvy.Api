using AnticevicApi.BL.Handlers.Movie;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AnticevicApi.Model.Binding.Movie;

namespace AnticevicApi.Controllers.Movie
{
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
