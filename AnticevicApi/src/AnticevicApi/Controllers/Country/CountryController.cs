using AnticevicApi.BL.Handlers.Country;
using AnticevicApi.Model.Binding.Country;
using AnticevicApi.Model.Constants.Database;
using AnticevicApi.Model.View;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using View = AnticevicApi.Model.View.Country;

namespace AnticevicApi.Controllers.Country
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
        [Route("{id}")]
        public View.Country Get(string id)
        {
            return _countryHandler.Get(id);
        }

        [HttpGet]
        [Route("visited")]
        [Authorize(Roles = UserRole.User)]
        public IEnumerable<View.Country> GetCountries()
        {
            return _countryHandler.GetVisited();
        }

        #endregion
    }
}
