using System.Threading.Tasks;
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
    public async Task<int> GetAverage([FromQuery] MovieGetBinding binding) => await _movieHandler.GetRuntimeAverage(binding);

    [HttpGet("Sum")]
    public async Task<int> GetSum([FromQuery] MovieGetBinding binding) => await _movieHandler.GetSum(binding, x => x.Runtime);
}
