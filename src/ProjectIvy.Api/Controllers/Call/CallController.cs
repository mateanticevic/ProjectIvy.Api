using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Call;
using ProjectIvy.Model.Binding.Call;

namespace ProjectIvy.Api.Controllers.Call;

public class CallController : BaseController<CallController>
{
    private readonly ICallHandler _callHandler;

    public CallController(ILogger<CallController> logger, ICallHandler callHandler) : base(logger)
    {
        _callHandler = callHandler;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] CallGetBinding binding) => Ok(await _callHandler.Get(binding));

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CallBinding binding) => Ok(await _callHandler.Create(binding));
}
