using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.City;
using ProjectIvy.Model.Binding.City;

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
    }
}