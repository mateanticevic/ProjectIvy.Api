using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Geohash;
using ProjectIvy.Model.Binding.Geohash;
using ProjectIvy.Model.Constants.Database;

namespace ProjectIvy.Api.Controllers.Device
{
    [Authorize(Roles = UserRole.User)]
    public class GeohashController : BaseController<GeohashController>
    {
        private readonly IGeohashHandler _geohashHandler;

        public GeohashController(ILogger<GeohashController> logger, IGeohashHandler geohashHandler) : base(logger) => _geohashHandler = geohashHandler;

        [HttpGet]
        public async Task<IEnumerable<string>> Get(GeohashGetBinding binding) => await _geohashHandler.GetGeohashes(binding);
    }
}
