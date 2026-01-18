using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Brand;
using ProjectIvy.Model.Binding.Brand;
using ProjectIvy.Model.View;
using ViewBrand = ProjectIvy.Model.View.Brand;

namespace ProjectIvy.Api.Controllers.Brand;

[Authorize(ApiScopes.BasicUser)]
public class BrandController : BaseController<BrandController>
{
    private readonly IBrandHandler _brandHandler;

    public BrandController(ILogger<BrandController> logger, IBrandHandler brandHandler) : base(logger)
    {
        _brandHandler = brandHandler;
    }

    [HttpGet("{id}")]
    public ViewBrand.Brand Get(string id) => _brandHandler.Get(id);

    [HttpGet]
    public async Task<PagedView<ViewBrand.Brand>> Get([FromQuery] BrandGetBinding binding)
        => await _brandHandler.Get(binding);
}
