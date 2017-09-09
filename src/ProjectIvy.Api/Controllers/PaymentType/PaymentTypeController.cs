using ProjectIvy.BL.Handlers.PaymentType;
using ProjectIvy.Model.Constants.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ProjectIvy.Api.Controllers.PaymentType
{
    [Authorize(Roles = UserRole.User)]
    [Route("[controller]")]
    public class PaymentTypeController : BaseController<PaymentTypeController>
    {
        private readonly IPaymentTypeHandler _paymentHandler;

        public PaymentTypeController(ILogger<PaymentTypeController> logger, IPaymentTypeHandler paymentHandler) : base(logger)
        {
            _paymentHandler = paymentHandler;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_paymentHandler.GetPaymentTypes());
        }
    }
}
