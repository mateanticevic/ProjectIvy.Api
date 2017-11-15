using ProjectIvy.Model.Binding.Common;
using ProjectIvy.Model.Binding.Expense;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.Expense;

namespace ProjectIvy.BL.Handlers.Expense
{
    public interface IExpenseHandler : IHandler
    {
        void AddFile(string expenseValueId, string fileValueId, ExpenseFileBinding binding);

        string Create(ExpenseBinding binding);

        bool Delete(string valueId);

        View.Expense Get(string expenseId);

        PagedView<View.Expense> Get(ExpenseGetBinding binding);

        IEnumerable<View.ExpenseFile> GetFiles(string expenseValueId);

        int Count(FilteredBinding binding);

        Task<IEnumerable<GroupedByMonth<decimal>>> GetGroupedByMonthSum(ExpenseSumGetBinding binding);

        Task<IEnumerable<GroupedByYear<decimal>>> GetGroupedByYearSum(ExpenseSumGetBinding binding);

        Task<IEnumerable<KeyValuePair<string, decimal>>> GetGroupedByTypeSum(ExpenseSumGetBinding binding);

        Task<decimal> GetSum(ExpenseSumGetBinding binding);

        bool Update(ExpenseBinding binding);
    }
}
