using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Expense;
using ProjectIvy.Model.View.PaymentProvider;
using System.Threading.Tasks;

namespace ProjectIvy.Api.Controllers.PaymentProvider
{
    [ApiController]
    public class PaymentProviderController : BaseController<PaymentProviderController>
    {
        private readonly IExpenseHandler _expenseHandler;

        public PaymentProviderController(ILogger<PaymentProviderController> logger, IExpenseHandler expenseHandler) : base(logger)
        {
            _expenseHandler = expenseHandler;
        }

        [HttpPost("transferWise/Notify")]
        public async Task<IActionResult> PostTransferWiseNotify([FromQuery] string authorizationCode, [FromBody] TransferWiseTransferEvent transferEvent)
        {
            if (string.IsNullOrWhiteSpace(transferEvent.Message))
                await _expenseHandler.NotifyTransferWiseEvent(authorizationCode, transferEvent.ResourceId);

            return Ok();
        }
    }
}