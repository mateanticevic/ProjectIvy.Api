using System.Threading.Tasks;
using ProjectIvy.Model.Binding.ExpenseType;
using ProjectIvy.Model.View;
using ProjectIvy.Model.View.Expense;
using ProjectIvy.Model.View.ExpenseType;

namespace ProjectIvy.Business.Handlers.Expense;

public interface IExpenseTypeHandler : IHandler
{
    Task<ExpenseType> Create(ExpenseTypeBinding binding);

    Task<IEnumerable<ExpenseType>> Get(ExpenseTypeGetBinding binding);

    Task<IEnumerable<ExpenseFileType>> GetFileTypes();

    Task<IEnumerable<Node<ExpenseType>>> GetTree();

    Task SetParent(string parentValueId, string childValueId);
}
