using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Expense;
using ProjectIvy.Model.Binding.ExpenseType;
using System.Collections.Generic;
using View = ProjectIvy.Model.View;

namespace ProjectIvy.Api.Controllers.ExpenseType
{
    [Route("[controller]")]
    public class ExpenseTypeController : BaseController<ExpenseTypeController>
    {
        private readonly IExpenseTypeHandler _expenseTypeHandler;

        public ExpenseTypeController(ILogger<ExpenseTypeController> logger, IExpenseTypeHandler expenseTypeHandler) : base(logger) => _expenseTypeHandler = expenseTypeHandler;

        #region Get

        [HttpGet]
        public IEnumerable<View.ExpenseType.ExpenseType> Get([FromQuery] ExpenseTypeGetBinding binding) => _expenseTypeHandler.Get(binding);

        [HttpGet("Tree")]
        public IEnumerable<View.Node<View.ExpenseType.ExpenseType>> GetTree() => _expenseTypeHandler.GetTree();

        #endregion
    }
}
