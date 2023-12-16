using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Income;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Income;
using ProjectIvy.Model.View;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.Income;

namespace ProjectIvy.Api.Controllers.Income
{
    [Authorize(ApiScopes.IncomeUser)]
    public class IncomeController : BaseController<IncomeController>
    {
        private readonly IIncomeHandler _incomeHandler;

        public IncomeController(ILogger<IncomeController> logger, IIncomeHandler incomeHandler) : base(logger) => _incomeHandler = incomeHandler;

        [HttpGet]
        public PagedView<View.Income> Get([FromQuery] IncomeGetBinding binding) => _incomeHandler.Get(binding);

        [HttpGet("Count")]
        public int GetCount([FromQuery] FilteredBinding binding) => _incomeHandler.GetCount(binding);

        [HttpGet("Source")]
        public async Task<IActionResult> GetSources() => Ok(await _incomeHandler.GetSources());

        [HttpGet("Sum")]
        public async Task<decimal> GetSum([FromQuery] IncomeGetSumBinding binding) => await _incomeHandler.GetSum(binding);

        [HttpGet("Sum/ByMonthOfYear")]
        public IActionResult GetSumByMonth([FromQuery] IncomeGetSumBinding binding) => Ok(_incomeHandler.GetSumByMonthOfYear(binding));

        [HttpGet("Sum/ByYear")]
        public IActionResult GetSumByYear([FromQuery] IncomeGetSumBinding binding) => Ok(_incomeHandler.GetSumByYear(binding));

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] IncomeBinding binding)
        {
            await _incomeHandler.Add(binding);
            return Ok();
        }
    }
}
