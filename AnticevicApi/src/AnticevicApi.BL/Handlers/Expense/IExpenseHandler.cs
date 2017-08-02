using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.Binding.Expense;
using AnticevicApi.Model.View;
using System.Collections.Generic;
using System.Threading.Tasks;
using View = AnticevicApi.Model.View.Expense;

namespace AnticevicApi.BL.Handlers.Expense
{
    public interface IExpenseHandler : IHandler
    {
        string Create(ExpenseBinding binding);

        bool Delete(string valueId);

        View.Expense Get(string expenseId);

        PagedView<View.Expense> Get(ExpenseGetBinding binding);

        int Count(FilteredBinding binding);

        Task<IEnumerable<GroupedByMonth<decimal>>> GetGroupedByMonthSum(ExpenseSumGetBinding binding);

        Task<IEnumerable<GroupedByYear<decimal>>> GetGroupedByYearSum(ExpenseSumGetBinding binding);

        Task<IEnumerable<KeyValuePair<string, decimal>>> GetGroupedByTypeSum(ExpenseSumGetBinding binding);

        Task<decimal> GetSum(ExpenseSumGetBinding binding);

        bool Update(ExpenseBinding binding);
    }
}
