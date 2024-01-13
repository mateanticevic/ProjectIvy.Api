using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Ride;
using ProjectIvy.Model.Binding.Route;
using ProjectIvy.Model.View;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProjectIvy.Api.Controllers.Route
{
    public class RouteController : BaseController<RouteController>
    {
        private readonly IRouteHandler _routeHandler;

        public RouteController(ILogger<RouteController> logger, IRouteHandler routeHandler) : base(logger)
        {
            _routeHandler = routeHandler;
        }

        [HttpGet]
        public async Task<PagedView<Model.View.Route.Route>> Get(RouteGetBinding  b) => await _routeHandler.GetRoutes(b);

        [HttpGet("{id}/Points")]
        public async Task<IEnumerable<decimal[]>> GetPoints(string id) => await _routeHandler.GetRoutePoints(id);

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RouteBinding binding)
        {
            await _routeHandler.Create(binding);
            return Ok();
        }

        [HttpPost("{id}/Kml")]
        public async Task<IActionResult> PostPoints(string id, [FromQuery] string kmlName)
        {
            var bytes = new byte[HttpContext.Request.ContentLength.Value];

            using var ms = new MemoryStream(bytes.Length);
            await HttpContext.Request.Body.CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);

            using var sr = new StreamReader(ms);

            var kml = XDocument.Parse(await sr.ReadToEndAsync());

            _routeHandler.SetPointsFromKml(id, kml, kmlName);
            return Ok();
        }
    }
}
