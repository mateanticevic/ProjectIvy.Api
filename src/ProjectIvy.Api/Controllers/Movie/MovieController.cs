using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Movie;
using ProjectIvy.Model.Binding.Movie;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Movie;

namespace ProjectIvy.Api.Controllers.Movie
{
    public class MovieController : BaseController<MovieController>
    {
        private readonly IMovieHandler _movieHandler;

        public MovieController(ILogger<MovieController> logger, IMovieHandler movieHandler) : base(logger)
        {
            _movieHandler = movieHandler;
        }

        [HttpGet]
        public PagedView<View.Movie> Get([FromQuery] MovieGetBinding binding) => _movieHandler.Get(binding);

        [HttpGet("{imdbId}")]
        public View.Movie Get(string imdbId) => _movieHandler.Get(imdbId);

        [HttpGet("Count")]
        public int GetCount([FromQuery] MovieGetBinding binding) => _movieHandler.Count(binding);

        [HttpGet("Count/ByDay")]
        public IActionResult GetCountByDay([FromQuery] MovieGetBinding binding) => Ok(_movieHandler.CountByDay(binding));

        [HttpGet("Count/ByDayOfWeek")]
        public IActionResult GetCountByDayOfWeek([FromQuery] MovieGetBinding binding) => Ok(_movieHandler.CountByDayOfWeek(binding));

        [HttpGet("Count/ByMonth")]
        public IActionResult GetCountByMonth([FromQuery] MovieGetBinding binding) => Ok(_movieHandler.CountByMonth(binding));

        [HttpGet("Count/ByMovieDecade")]
        public IActionResult GetCountByMovieDecade([FromQuery] MovieGetBinding binding) => Ok(_movieHandler.CountByMovieDecade(binding));

        [HttpGet("Count/ByMovieYear")]
        public IActionResult GetCountByMovieYear([FromQuery] MovieGetBinding binding) => Ok(_movieHandler.CountByMovieYear(binding));

        [HttpGet("Count/ByMyRating")]
        public IActionResult GetCountByMyRating([FromQuery] MovieGetBinding binding) => Ok(_movieHandler.CountByMyRating(binding));

        [HttpGet("Count/ByMonthOfYear")]
        public IActionResult GetCountByMonthOfYear([FromQuery] MovieGetBinding binding) => Ok(_movieHandler.CountByMonthOfYear(binding));

        [HttpGet("Count/ByRuntime")]
        public IActionResult GetCountByRuntime([FromQuery] MovieGetBinding binding) => Ok(_movieHandler.CountByRuntime(binding));

        [HttpGet("Count/ByYear")]
        public IActionResult GetCountByYear([FromQuery] MovieGetBinding binding) => Ok(_movieHandler.CountByYear(binding));
    }
}
