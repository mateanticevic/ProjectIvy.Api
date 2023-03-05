using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Expense;

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
    }
}