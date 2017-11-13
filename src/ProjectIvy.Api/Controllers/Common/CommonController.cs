using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.Currency;
using ProjectIvy.BL.Handlers.Expense;
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
        private readonly IPoiHandler _poiHandler;

        public CommonController(ILogger<CommonController> logger, ICurrencyHandler currencyHandler, IPoiHandler poiHandler, IExpenseTypeHandler expenseTypeHandler) : base(logger)
        {
            _currencyHandler = currencyHandler;
            _expenseTypeHandler = expenseTypeHandler;
            _poiHandler = poiHandler;
        }

        [HttpGet]
        [Route("currency")]
        public IEnumerable<View.Currency.Currency> GetCurrencies()
        {
            return _currencyHandler.Get();
        }

        [HttpGet]
        [Route("expenseFileType")]
        public IEnumerable<View.Expense.ExpenseFileType> GetExpenseFileTypes()
        {
            return _expenseTypeHandler.GetFileTypes();
        }

        [HttpGet]
        [Route("poiCategory")]
        public IEnumerable<View.Poi.PoiCategory> GetPoiCategories()
        {
            return _poiHandler.GetCategories();
        }
    }
}