using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.City;
using ProjectIvy.Model.Binding.City;
using ProjectIvy.Model.Constants.Database;
using View = ProjectIvy.Model.View.City;

namespace ProjectIvy.Api.Controllers.City
{
    public class CityController : BaseController<CityController>
    {
        private readonly ICityHandler _cityHandler;

        public CityController(ILogger<CityController> logger, ICityHandler cityHandler) : base(logger)
        {
            _cityHandler = cityHandler;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] CityGetBinding binding) => Ok(await _cityHandler.Get(binding));

        [HttpGet("Visited")]
        [Authorize(Roles = UserRole.User)]
        public IEnumerable<View.City> GetVisited() => _cityHandler.GetVisited();
    }
}