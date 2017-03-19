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

        IEnumerable<View.Expense> GetByType(string valueId, DateTime? from = default(DateTime?), DateTime? to = default(DateTime?));

        IEnumerable<View.Expense> GetByVendor(string valueId, DateTime? from = default(DateTime?), DateTime? to = default(DateTime?));

        int GetCount(FilteredBinding binding);

        Task<IEnumerable<GroupedByMonth<decimal>>> GetGroupedByMonthSum(ExpenseSumGetBinding binding);

        Task<IEnumerable<GroupedByYear<decimal>>> GetGroupedByYearSum(ExpenseSumGetBinding binding);

        Task<decimal> GetSum(ExpenseSumGetBinding binding);

        bool Update(ExpenseBinding binding);
    }
}
