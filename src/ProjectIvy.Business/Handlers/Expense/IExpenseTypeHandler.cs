using System.Collections.Generic;
using ProjectIvy.Model.Binding.ExpenseType;
using ProjectIvy.Model.View;
using ProjectIvy.Model.View.Expense;
using ProjectIvy.Model.View.ExpenseType;

namespace ProjectIvy.Business.Handlers.Expense;

public interface IExpenseTypeHandler : IHandler
{
    IEnumerable<ExpenseType> Get(ExpenseTypeGetBinding binding);

    IEnumerable<ExpenseFileType> GetFileTypes();

    IEnumerable<Node<ExpenseType>> GetTree();
}
