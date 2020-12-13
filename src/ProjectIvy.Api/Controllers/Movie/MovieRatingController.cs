using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Movie;
using ProjectIvy.Model.Binding.Movie;
using ProjectIvy.Model.Constants.Database;

namespace ProjectIvy.Api.Controllers.Movie
{
    [Authorize(Roles = UserRole.User)]
    [Route("Movie/Rating")]
    public class MovieRatingController : BaseController<MovieController>
    {
        private readonly IMovieHandler _movieHandler;

        public MovieRatingController(ILogger<MovieController> logger, IMovieHandler movieHandler) : base(logger)
        {
            _movieHandler = movieHandler;
        }

        [HttpGet("Average")]
        public double GetAverage([FromQuery] MovieGetBinding binding) => _movieHandler.GetRatingAverage(binding);

        [HttpGet("ByYear")]
        public async Task<IActionResult> GetAveragerByYear([FromQuery] MovieGetBinding binding) => Ok(await _movieHandler.GetRatingAverageByYear(binding));
    }
}
