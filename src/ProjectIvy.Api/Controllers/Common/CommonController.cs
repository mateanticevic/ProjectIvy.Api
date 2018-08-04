using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.Currency;
using ProjectIvy.BL.Handlers.Expense;
using ProjectIvy.BL.Handlers.PaymentType;
using ProjectIvy.BL.Handlers.Poi;
using System.Collections.Generic;
using ProjectIvy.BL.Handlers.Beer;
using View = ProjectIvy.Model.View;

namespace ProjectIvy.Api.Controllers.Common
{
    [Route("[controller]")]
    public class CommonController : BaseController<CommonController>
    {
        private readonly ICurrencyHandler _currencyHandler;
        private readonly IBeerHandler _beerHandler;
        private readonly IExpenseTypeHandler _expenseTypeHandler;
        private readonly IPaymentTypeHandler _paymentHandler;
        private readonly IPoiHandler _poiHandler;

        public CommonController(ILogger<CommonController> logger,
                                ICurrencyHandler currencyHandler,
                                IBeerHandler beerHandler,
                                IPaymentTypeHandler paymentTypeHandler,
                                IPoiHandler poiHandler,
                                IExpenseTypeHandler expenseTypeHandler) : base(logger)
        {
            _beerHandler = beerHandler;
            _currencyHandler = currencyHandler;
            _expenseTypeHandler = expenseTypeHandler;
            _paymentHandler = paymentTypeHandler;
            _poiHandler = poiHandler;
        }

        [HttpGet("Currency")]
        public IEnumerable<View.Currency.Currency> GetCurrencies() => _currencyHandler.Get();

        [HttpGet("BeerServing")]
        public IActionResult GetBeerServings() => Ok(_beerHandler.GetServings());

        [HttpGet("ExpenseFileType")]
        public IEnumerable<View.Expense.ExpenseFileType> GetExpenseFileTypes() => _expenseTypeHandler.GetFileTypes();

        [HttpGet("PaymentType")]
        public IActionResult GetPaymentTypes() => Ok(_paymentHandler.GetPaymentTypes());

        [HttpGet("PoiCategory")]
        public IEnumerable<View.Poi.PoiCategory> GetPoiCategories() => _poiHandler.GetCategories();
    }
}