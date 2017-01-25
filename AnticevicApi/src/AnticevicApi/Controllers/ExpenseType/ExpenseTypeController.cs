using AnticevicApi.BL.Handlers.Expense;
using AnticevicApi.Model.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using ViewExpense = AnticevicApi.Model.View.Expense;
using ViewExpenseType = AnticevicApi.Model.View.ExpenseType;

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
        public IEnumerable<ViewExpenseType.ExpenseType> Get()
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Get));

            return _expenseTypeHandler.Get();
        }

        [HttpGet]
        [Route("{valueId}/expense")]
        public IEnumerable<ViewExpense.Expense> GetExpenses(string valueId, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetExpenses), valueId, from, to);

            return _expenseHandler.GetByType(valueId, from, to);
        }

        #endregion
    }
}
