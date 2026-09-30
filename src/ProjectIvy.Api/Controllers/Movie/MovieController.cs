using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Movie;
using ProjectIvy.Model.Binding.Movie;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Movie;

namespace ProjectIvy.Api.Controllers.Movie;

[Authorize(ApiScopes.MovieUser)]
public class MovieController : BaseController<MovieController>
{
    private readonly IMovieHandler _movieHandler;

    public MovieController(ILogger<MovieController> logger, IMovieHandler movieHandler) : base(logger)
    {
        _movieHandler = movieHandler;
    }

    [HttpGet]
    public async Task<PagedView<View.Movie>> Get([FromQuery] MovieGetBinding binding) => await _movieHandler.Get(binding);

    [HttpGet("{imdbId}")]
    public async Task<View.Movie> Get(string imdbId) => await _movieHandler.Get(imdbId);

    [HttpGet("Count")]
    public async Task<int> GetCount([FromQuery] MovieGetBinding binding) => await _movieHandler.Count(binding);

    [HttpGet("Count/ByDay")]
    public async Task<IActionResult> GetCountByDay([FromQuery] MovieGetBinding binding) => Ok(await _movieHandler.CountByDay(binding));

    [HttpGet("Count/ByDayOfWeek")]
    public async Task<IActionResult> GetCountByDayOfWeek([FromQuery] MovieGetBinding binding) => Ok(await _movieHandler.CountByDayOfWeek(binding));

    [HttpGet("Count/ByMonth")]
    public async Task<IActionResult> GetCountByMonth([FromQuery] MovieGetBinding binding) => Ok(await _movieHandler.CountByMonth(binding));

    [HttpGet("Count/ByMonthOfYear")]
    public async Task<IActionResult> GetCountByMonthOfYear([FromQuery] MovieGetBinding binding) => Ok(await _movieHandler.CountByMonthOfYear(binding));

    [HttpGet("Count/ByMovieDecade")]
    public async Task<IActionResult> GetCountByMovieDecade([FromQuery] MovieGetBinding binding) => Ok(await _movieHandler.CountByMovieDecade(binding));

    [HttpGet("Count/ByMovieYear")]
    public async Task<IActionResult> GetCountByMovieYear([FromQuery] MovieGetBinding binding) => Ok(await _movieHandler.CountByMovieYear(binding));

    [HttpGet("Count/ByMyRating")]
    public async Task<IActionResult> GetCountByMyRating([FromQuery] MovieGetBinding binding) => Ok(await _movieHandler.CountByMyRating(binding));

    [HttpGet("Count/ByRuntime")]
    public async Task<IActionResult> GetCountByRuntime([FromQuery] MovieGetBinding binding) => Ok(await _movieHandler.CountByRuntime(binding));

    [HttpGet("Count/ByYear")]
    public async Task<IActionResult> GetCountByYear([FromQuery] MovieGetBinding binding) => Ok(await _movieHandler.CountByYear(binding));
}
