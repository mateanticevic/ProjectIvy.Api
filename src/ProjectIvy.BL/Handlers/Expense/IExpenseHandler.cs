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

        int Count(FilteredBinding binding);

        int CountTypes(ExpenseGetBinding binding);

        int CountVendors(ExpenseGetBinding binding);

        string Create(ExpenseBinding binding);

        bool Delete(string valueId);

        View.Expense Get(string expenseId);

        PagedView<View.Expense> Get(ExpenseGetBinding binding);

        IEnumerable<View.ExpenseFile> GetFiles(string expenseValueId);

        Task<IEnumerable<GroupedByMonth<decimal>>> GetGroupedByMonthSum(ExpenseSumGetBinding binding);

        Task<IEnumerable<GroupedByYear<decimal>>> GetGroupedByYearSum(ExpenseSumGetBinding binding);

        Task<IEnumerable<KeyValuePair<string, decimal>>> GetGroupedByTypeSum(ExpenseSumGetBinding binding);

        Task<decimal> GetSum(ExpenseSumGetBinding binding);

        bool Update(ExpenseBinding binding);
    }
}
