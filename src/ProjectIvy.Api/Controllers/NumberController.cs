using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Call;
using System.Threading.Tasks;

namespace ProjectIvy.Api.Controllers
{
    [ApiController]
    public class NumberController : BaseController<NumberController>
    {
        private readonly ICallHandler _callHandler;

        public NumberController(ILogger<NumberController> logger, ICallHandler callHandler) : base(logger)
        {
            _callHandler = callHandler;
        }

        [HttpGet("{number}/IsBlacklisted")]
        public async Task<IActionResult> GetIsBlacklisted(string number) => Ok(await _callHandler.IsNumberBlacklisted(number));
    }
}