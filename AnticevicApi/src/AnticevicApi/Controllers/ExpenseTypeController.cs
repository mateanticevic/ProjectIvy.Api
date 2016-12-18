using AnticevicApi.BL.Handlers.Expense;
using AnticevicApi.Model.Constants;
using AnticevicApi.Model.View.Expense;
using AnticevicApi.Model.View.ExpenseType;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class ExpenseTypeController : BaseController<ExpenseTypeController>
    {
        private readonly IExpenseHandler _expenseHandler;
        private readonly IExpenseTypeHandler _expenseTypeHandler;

        public ExpenseTypeController(ILogger<ExpenseTypeController> logger, IExpenseHandler expenseHandler, IExpenseTypeHandler expenseTypeHandler) : base(logger)
        {
            _expenseHandler = expenseHandler;
            _expenseTypeHandler = expenseTypeHandler;
        }

        #region Get

        [HttpGet]
        public IEnumerable<ExpenseType> Get()
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Get));

            return _expenseTypeHandler.Get();
        }

        [HttpGet]
        [Route("{valueId}/expense")]
        public IEnumerable<Expense> GetExpenses(string valueId, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetExpenses), valueId, from, to);

            return _expenseHandler.GetByType(valueId, from, to);
        }

        #endregion
    }
}
