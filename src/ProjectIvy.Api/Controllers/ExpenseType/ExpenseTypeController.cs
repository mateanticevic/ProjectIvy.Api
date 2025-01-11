using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Expense;
using ProjectIvy.Model.Binding.ExpenseType;
using View = ProjectIvy.Model.View;

namespace ProjectIvy.Api.Controllers.ExpenseType;

[Authorize(ApiScopes.ExpenseUser)]
public class ExpenseTypeController : BaseController<ExpenseTypeController>
{
    private readonly IExpenseTypeHandler _expenseTypeHandler;

    public ExpenseTypeController(ILogger<ExpenseTypeController> logger, IExpenseTypeHandler expenseTypeHandler) : base(logger) => _expenseTypeHandler = expenseTypeHandler;

    [HttpGet]
    public IEnumerable<View.ExpenseType.ExpenseType> Get([FromQuery] ExpenseTypeGetBinding binding) => _expenseTypeHandler.Get(binding);

    [HttpGet("Tree")]
    public IEnumerable<View.Node<View.ExpenseType.ExpenseType>> GetTree() => _expenseTypeHandler.GetTree();
}
