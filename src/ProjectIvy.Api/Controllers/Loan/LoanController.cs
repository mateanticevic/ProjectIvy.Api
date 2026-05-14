using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Loan;
using ProjectIvy.Model.Binding.Loan;

namespace ProjectIvy.Api.Controllers.Loan;

[Authorize(ApiScopes.BasicUser)]
public class LoanController : BaseController<LoanController>
{
    private readonly ILoanHandler _loanHandler;

    public LoanController(ILogger<LoanController> logger, ILoanHandler loanHandler) : base(logger)
    {
        _loanHandler = loanHandler;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] LoanGetBinding binding) => Ok(await _loanHandler.Get(binding));
}
