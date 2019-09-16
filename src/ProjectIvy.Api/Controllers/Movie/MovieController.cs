using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Movie;
using ProjectIvy.Model.Binding.Movie;
using ProjectIvy.Model.Constants.Database;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using View = ProjectIvy.Model.View.Movie;

namespace ProjectIvy.Api.Controllers.Movie
{
    [Authorize(Roles = UserRole.User)]
    public class MovieController : BaseController<MovieController>
    {
        private readonly IMovieHandler _movieHandler;

        public MovieController(ILogger<MovieController> logger, IMovieHandler movieHandler) : base(logger)
        {
            _movieHandler = movieHandler;
        }

        #region Get

        [HttpGet]
        public PagedView<View.Movie> Get([FromQuery] MovieGetBinding binding) => _movieHandler.Get(binding);

        [HttpGet("{imdbId}")]
        public View.Movie Get(string imdbId) => _movieHandler.Get(imdbId);

        [HttpGet("Count")]
        public int GetCount([FromQuery] MovieGetBinding binding) => _movieHandler.Count(binding);

        [HttpGet("Count/ByMonth")]
        public IEnumerable<GroupedByMonth<int>> GetCountByMonth([FromQuery] MovieGetBinding binding) => _movieHandler.CountByMonth(binding);

        [HttpGet("Count/ByYear")]
        public IEnumerable<GroupedByYear<int>> GetCountByYear([FromQuery] MovieGetBinding binding) => _movieHandler.CountByYear(binding);

        #endregion
    }
}
