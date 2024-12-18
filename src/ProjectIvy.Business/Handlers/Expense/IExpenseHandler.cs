﻿using ProjectIvy.Model.Binding.Expense;
using ProjectIvy.Model.Binding.File;
using ProjectIvy.Model.View;
using ProjectIvy.Model.View.ExpenseType;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.Expense;

namespace ProjectIvy.Business.Handlers.Expense
{
    public interface IExpenseHandler : IHandler
    {
        void AddFile(string expenseValueId, string fileValueId, ExpenseFileBinding binding);

        int Count(ExpenseGetBinding binding);

        IEnumerable<KeyValuePair<string, int>> CountByDay(ExpenseGetBinding binding);

        IEnumerable<KeyValuePair<int, int>> CountByDayOfWeek(ExpenseGetBinding binding);

        IEnumerable<KeyValuePair<int, int>> CountByMonth(ExpenseGetBinding binding);

        IEnumerable<KeyValuePair<string, int>> CountByMonthOfYear(ExpenseGetBinding binding);

        IEnumerable<KeyValuePair<int, int>> CountByYear(ExpenseGetBinding binding);

        PagedView<KeyValuePair<ExpenseType, int>> CountByType(ExpenseGetBinding binding);

        PagedView<KeyValuePair<Model.View.Vendor.Vendor, int>> CountByVendor(ExpenseGetBinding binding);

        int CountTypes(ExpenseGetBinding binding);

        int CountVendors(ExpenseGetBinding binding);

        string Create(ExpenseBinding binding);

        Task CreateFromFile(FileBinding binding);

        bool Delete(string valueId);

        View.Expense Get(string expenseId);

        PagedView<View.Expense> Get(ExpenseGetBinding binding);

        IEnumerable<View.ExpenseFile> GetFiles(string expenseValueId);

        Task<IEnumerable<string>> GetTopDescriptions(ExpenseGetBinding binding);

        Task<IEnumerable<KeyValuePair<Model.View.Currency.Currency, decimal>>> SumByCurrency(ExpenseSumGetBinding binding);

        Task<decimal> SumAmount(ExpenseSumGetBinding binding);

        Task<IEnumerable<KeyValuePair<DateTime, decimal>>> SumAmountByDay(ExpenseSumGetBinding binding);

        Task<IEnumerable<KeyValuePair<int, decimal>>> SumAmountByDayOfWeek(ExpenseSumGetBinding binding);

        Task<IEnumerable<KeyValuePair<int, decimal>>> SumAmountByMonth(ExpenseSumGetBinding binding);

        IEnumerable<KeyValuePair<string, decimal>> SumAmountByMonthOfYear(ExpenseSumGetBinding binding);

        Task<IEnumerable<KeyValuePair<string, IEnumerable<KeyValuePair<string, decimal>>>>> SumByMonthOfYearByType(ExpenseSumGetBinding binding);

        IEnumerable<KeyValuePair<int, decimal>> SumAmountByYear(ExpenseSumGetBinding binding);

        Task<IEnumerable<KeyValuePair<short, IEnumerable<KeyValuePair<string, decimal>>>>> SumByYearByType(ExpenseSumGetBinding binding);

        Task<IEnumerable<KeyValuePair<string, decimal>>> SumByType(ExpenseSumGetBinding binding);

        bool Update(ExpenseBinding binding);
    }
}
