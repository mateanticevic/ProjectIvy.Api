using AnticevicApi.BL.Handlers.Expense;
using AnticevicApi.Config;
using AnticevicApi.Model.View.Expense;
using AnticevicApi.Model.View.ExpenseType;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class ExpenseTypeController : BaseController<ExpenseTypeController>
    {
        public ExpenseTypeController(IOptions<AppSettings> options, ILogger<ExpenseTypeController> logger, IExpenseTypeHandler expenseTypeHandler) : base(options, logger)
        {
            ExpenseTypeHandler = expenseTypeHandler;
        }

        #region Get

        [HttpGet]
        public IEnumerable<ExpenseType> Get()
        {
            return ExpenseTypeHandler.Get();
        }

        [HttpGet]
        [Route("{valueId}/expense")]
        public IEnumerable<Expense> GetExpenses(string valueId, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            return ExpenseHandler.GetByType(valueId, from, to);
        }

        #endregion
    }
}
