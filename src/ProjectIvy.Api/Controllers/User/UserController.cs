using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.User;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.User;
using View = ProjectIvy.Model.View.User;

namespace ProjectIvy.Api.Controllers.User;

[Authorize(ApiScopes.BasicUser)]
public class UserController : BaseController<UserController>
{
    private readonly IUserHandler _userHandler;

    public UserController(ILogger<UserController> logger, IUserHandler userHandler) : base(logger) => _userHandler = userHandler;

    [HttpGet]
    [ResponseCache(Duration = 10)]
    public View.User Get() => _userHandler.Get();

    [HttpGet("Weight")]
    public async Task<IEnumerable<KeyValuePair<DateTime, decimal>>> GetWeight([FromQuery] FilteredBinding b) => await _userHandler.GetWeight(b);

    [HttpPost("Weight")]
    public async Task<IActionResult> PostWeight([FromBody] WeightBinding binding)
    {
        await _userHandler.AddWeight(binding);
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] UserUpdateBinding binding)
    {
        await _userHandler.Update(binding);
        return Ok();
    }
}
