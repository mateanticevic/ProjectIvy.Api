using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Bank;
using ProjectIvy.Model.Binding.Bank;

namespace ProjectIvy.Api.Controllers.Bank;

[Authorize(ApiScopes.BasicUser)]
public class BankController : BaseController<BankController>
{
    private readonly IBankHandler _bankHandler;

    public BankController(ILogger<BankController> logger, IBankHandler bankHandler) : base(logger)
    {
        _bankHandler = bankHandler;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] BankGetBinding binding) => Ok(await _bankHandler.Get(binding));
}
