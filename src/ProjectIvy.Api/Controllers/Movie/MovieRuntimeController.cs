using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Movie;
using ProjectIvy.Model.Binding.Movie;

namespace ProjectIvy.Api.Controllers.Movie;

[Route("Movie/Runtime")]
[Authorize(ApiScopes.MovieUser)]
public class MovieRuntimeController : BaseController<MovieController>
{
    private readonly IMovieHandler _movieHandler;

    public MovieRuntimeController(ILogger<MovieController> logger, IMovieHandler movieHandler) : base(logger)
    {
        _movieHandler = movieHandler;
    }

    [HttpGet("Average")]
    public int GetAverage([FromQuery] MovieGetBinding binding) => _movieHandler.GetRuntimeAverage(binding);

    [HttpGet("Sum")]
    public int GetSum([FromQuery] MovieGetBinding binding) => _movieHandler.GetSum(binding, x => x.Runtime);
}
