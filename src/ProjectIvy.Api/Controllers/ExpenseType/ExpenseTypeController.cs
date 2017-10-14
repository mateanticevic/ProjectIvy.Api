using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.Expense;
using ProjectIvy.Model.Binding.ExpenseType;
using System.Collections.Generic;
using View = ProjectIvy.Model.View;

namespace ProjectIvy.Api.Controllers
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
        public IEnumerable<View.ExpenseType.ExpenseType> Get([FromQuery] ExpenseTypeGetBinding binding)
        {
            return _expenseTypeHandler.Get(binding);
        }

        [HttpGet]
        [Route("tree")]
        public IEnumerable<View.Node<View.ExpenseType.ExpenseType>> GetTree()
        {
            return _expenseTypeHandler.GetTree();
        }

        #endregion
    }
}
