using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Country;
using ProjectIvy.Model.Binding.Country;
using ProjectIvy.Model.Constants.Database;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using View = ProjectIvy.Model.View.Country;

namespace ProjectIvy.Api.Controllers.Country
{
    public class CountryController : BaseController<CountryController>
    {
        private readonly ICountryHandler _countryHandler;

        public CountryController(ILogger<CountryController> logger, ICountryHandler countryHandler) : base(logger) => _countryHandler = countryHandler;

        #region Get

        [HttpGet]
        public PagedView<View.Country> Get(CountryGetBinding binding) => _countryHandler.Get(binding);

        [HttpGet("Count")]
        public long GetCount(CountryGetBinding binding) => _countryHandler.Count(binding);

        [HttpGet("{id}")]
        public View.Country Get(string id) => _countryHandler.Get(id);

        [HttpGet("Visited")]
        [Authorize(Roles = UserRole.User)]
        public IEnumerable<View.Country> GetVisited() => _countryHandler.GetVisited();

        [HttpGet("Visited/Count")]
        public long GetVisitedCount() => _countryHandler.CountVisited();

        [HttpGet("Visited/Boundaries")]
        public IEnumerable<View.CountryBoundaries> GetVisitedBoundaries() => _countryHandler.GetBoundaries(_countryHandler.GetVisited());

        #endregion
    }
}
