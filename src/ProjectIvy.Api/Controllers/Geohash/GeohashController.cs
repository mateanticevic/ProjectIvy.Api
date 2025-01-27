﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Geohash;
using ProjectIvy.Model.Binding.Geohash;

namespace ProjectIvy.Api.Controllers.Device;

public class GeohashController : BaseController<GeohashController>
{
    private readonly IGeohashHandler _geohashHandler;

    public GeohashController(ILogger<GeohashController> logger, IGeohashHandler geohashHandler) : base(logger) => _geohashHandler = geohashHandler;

    [HttpDelete("{geohash}/Trackings")]
    public async Task Delete(string geohash) => await _geohashHandler.DeleteTrackings(geohash);

    [HttpGet]
    public async Task<IActionResult> Get(GeohashGetBinding binding) => Ok(await _geohashHandler.GetGeohashes(binding));

    [HttpGet("Unique")]
    public async Task<IEnumerable<string>> GetUnique(GeohashUniqueGetBinding binding) => await _geohashHandler.GetUnique(binding);

    [HttpGet("Unique/Count")]
    public async Task<int> GetUniqueCount(GeohashUniqueGetBinding binding) => await _geohashHandler.CountUnique(binding);

    [HttpGet("{geohash}")]
    public async Task<IActionResult> GetGeohash(string geohash)
    {
        var view = await _geohashHandler.GetGeohash(geohash);

        return view is null ? NotFound() : Ok(view);
    }

    [HttpGet("{geohash}/Days")]
    public async Task<IActionResult> GetDays(string geohash) => Ok(await _geohashHandler.GetDays(geohash));

    [HttpGet("{fromGeohash}/To/{toGeohash}")]
    public async Task<IActionResult> GetGeohashToGeohash(string fromGeohash, string toGeohash, [FromQuery] RouteTimeSort orderBy = RouteTimeSort.Date)
        => Ok(await _geohashHandler.FromGeohashToGeohash(new[] { fromGeohash }, new[] { toGeohash }, orderBy));

    [HttpGet("Root/Children")]
    public async Task<IActionResult> GetRootChildren([FromQuery] GeohashChildrenGetBinding binding) => Ok(await _geohashHandler.GetChildren(null, binding));

    [HttpGet("{geohash}/Children")]
    public async Task<IActionResult> GetChildren(string geohash, [FromQuery] GeohashChildrenGetBinding binding) => Ok(await _geohashHandler.GetChildren(geohash, binding));

    [HttpGet("{geohash}/City")]
    public async Task<IActionResult> GetCity(string geohash) => Ok(await _geohashHandler.GetCity(geohash));

    [HttpGet("{geohash}/Country")]
    public async Task<IActionResult> GetCountry(string geohash) => Ok(await _geohashHandler.GetCountry(geohash));

    [HttpGet("Route")]
    public async Task<IActionResult> GetRoutes([FromQuery] RouteGetBinding b)
        => Ok(await _geohashHandler.FromGeohashToGeohash(b.From, b.To, b.OrderBy));
}
