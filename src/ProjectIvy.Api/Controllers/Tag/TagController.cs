using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Tag;
using ProjectIvy.Model.Binding.Tag;
using ProjectIvy.Model.View;

namespace ProjectIvy.Api.Controllers.Tag;

[Authorize(ApiScopes.BasicUser)]
public class TagController : BaseController<TagController>
{
    private readonly ITagHandler _tagHandler;

    public TagController(ILogger<TagController> logger, ITagHandler tagHandler) : base(logger)
    {
        _tagHandler = tagHandler;
    }

    [HttpGet]
    public async Task<PagedView<Model.View.Tag.Tag>> Get([FromQuery] TagGetBinding binding) => await _tagHandler.Get(binding);

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TagBinding binding) => Ok(await _tagHandler.Create(binding));
}
