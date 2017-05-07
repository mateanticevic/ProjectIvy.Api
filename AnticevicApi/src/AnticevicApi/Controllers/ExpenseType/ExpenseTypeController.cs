using AnticevicApi.BL.Handlers.Expense;
using AnticevicApi.Model.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using ViewExpenseType = AnticevicApi.Model.View.ExpenseType;
using AnticevicApi.Model.Binding.ExpenseType;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class ExpenseTypeController : BaseController<ExpenseTypeController>
    {
        private readonly IExpenseTypeHandler _expenseTypeHandler;

        public ExpenseTypeController(ILogger<ExpenseTypeController> logger, IExpenseTypeHandler expenseTypeHandler) : base(logger)
        {
            _expenseTypeHandler = expenseTypeHandler;
        }

        #region Get

        [HttpGet]
        public IEnumerable<ViewExpenseType.ExpenseType> Get([FromQuery] ExpenseTypeGetBinding binding)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Get));

            return _expenseTypeHandler.Get(binding);
        }

        #endregion
    }
}
