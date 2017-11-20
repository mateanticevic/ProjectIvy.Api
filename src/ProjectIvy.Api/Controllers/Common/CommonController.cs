using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.Currency;
using ProjectIvy.BL.Handlers.Expense;
using ProjectIvy.BL.Handlers.PaymentType;
using ProjectIvy.BL.Handlers.Poi;
using System.Collections.Generic;
using View = ProjectIvy.Model.View;

namespace ProjectIvy.Api.Controllers.Poi
{
    [Route("[controller]")]
    public class CommonController : BaseController<CommonController>
    {
        private readonly ICurrencyHandler _currencyHandler;
        private readonly IExpenseTypeHandler _expenseTypeHandler;
        private readonly IPaymentTypeHandler _paymentHandler;
        private readonly IPoiHandler _poiHandler;

        public CommonController(ILogger<CommonController> logger, ICurrencyHandler currencyHandler,
                                                                  IPaymentTypeHandler paymentTypeHandler,
                                                                  IPoiHandler poiHandler,
                                                                  IExpenseTypeHandler expenseTypeHandler) : base(logger)
        {
            _currencyHandler = currencyHandler;
            _expenseTypeHandler = expenseTypeHandler;
            _paymentHandler = paymentTypeHandler;
            _poiHandler = poiHandler;
        }

        [HttpGet("Currency")]
        public IEnumerable<View.Currency.Currency> GetCurrencies()
        {
            return _currencyHandler.Get();
        }

        [HttpGet("ExpenseFileType")]
        public IEnumerable<View.Expense.ExpenseFileType> GetExpenseFileTypes()
        {
            return _expenseTypeHandler.GetFileTypes();
        }

        [HttpGet("PaymentType")]
        public IActionResult GetPaymentTypes()
        {
            return Ok(_paymentHandler.GetPaymentTypes());
        }

        [HttpGet("PoiCategory")]
        public IEnumerable<View.Poi.PoiCategory> GetPoiCategories()
        {
            return _poiHandler.GetCategories();
        }
    }
}