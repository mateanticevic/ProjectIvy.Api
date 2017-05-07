using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.Binding.Expense;
using AnticevicApi.Model.View;
using System.Collections.Generic;
using System;
using View = AnticevicApi.Model.View.Expense;
using System.Threading.Tasks;

namespace AnticevicApi.BL.Handlers.Expense
{
    public interface IExpenseHandler : IHandler
    {
        string Create(ExpenseBinding binding);

        bool Delete(string valueId);

        PaginatedView<View.Expense> Get(ExpenseGetBinding binding);

        IEnumerable<View.Expense> GetByDate(DateTime date);

        int Count(FilteredBinding binding);

        Task<IEnumerable<GroupedByMonth<decimal>>> GetGroupedByMonthSum(ExpenseSumGetBinding binding);

        Task<IEnumerable<GroupedByYear<decimal>>> GetGroupedByYearSum(ExpenseSumGetBinding binding);

        Task<IEnumerable<KeyValuePair<string, decimal>>> GetGroupedByTypeSum(ExpenseSumGetBinding binding);

        Task<decimal> GetSum(ExpenseSumGetBinding binding);

        bool Update(ExpenseBinding binding);
    }
}
