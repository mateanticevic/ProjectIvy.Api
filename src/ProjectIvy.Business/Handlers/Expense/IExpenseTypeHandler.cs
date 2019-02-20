using ProjectIvy.Model.Binding.ExpenseType;
using ProjectIvy.Model.View.ExpenseType;
using ProjectIvy.Model.View.Expense;
using ProjectIvy.Model.View;
using System.Collections.Generic;

namespace ProjectIvy.Business.Handlers.Expense
{
    public interface IExpenseTypeHandler : IHandler
    {
        IEnumerable<ExpenseType> Get(ExpenseTypeGetBinding binding);

        IEnumerable<ExpenseFileType> GetFileTypes();

        IEnumerable<Node<ExpenseType>> GetTree();
    }
}
