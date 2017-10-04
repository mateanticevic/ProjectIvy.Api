using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.Country;
using ProjectIvy.Model.Binding.Country;
using ProjectIvy.Model.Constants.Database;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using View = ProjectIvy.Model.View.Country;

namespace ProjectIvy.Api.Controllers.Country
{
    [Route("[controller]")]
    public class CountryController : BaseController<CountryController>
    {
        private readonly ICountryHandler _countryHandler;

        public CountryController(ILogger<CountryController> logger, ICountryHandler countryHandler) : base(logger)
        {
            _countryHandler = countryHandler;
        }

        #region Get

        [HttpGet]
        [Route("")]
        public PagedView<View.Country> Get(CountryGetBinding binding)
        {
            return _countryHandler.Get(binding);
        }

        [HttpGet]
        [Route("count")]
        public long GetCount(CountryGetBinding binding)
        {
            return _countryHandler.Count(binding);
        }

        [HttpGet]
        [Route("{id}")]
        public View.Country Get(string id)
        {
            return _countryHandler.Get(id);
        }

        [HttpGet]
        [Route("visited")]
        [Authorize(Roles = UserRole.User)]
        public IEnumerable<View.Country> GetVisited()
        {
            return _countryHandler.GetVisited();
        }

        [HttpGet]
        [Route("visited/count")]
        public long GetVisitedCount()
        {
            return _countryHandler.CountVisited();
        }

        [HttpGet]
        [Route("visited/boundaries")]
        public IEnumerable<View.CountryBoundaries> GetVisitedBoundaries()
        {
            var visitedCountries = _countryHandler.GetVisited();

            return _countryHandler.GetBoundaries(visitedCountries);
        }

        #endregion
    }
}
