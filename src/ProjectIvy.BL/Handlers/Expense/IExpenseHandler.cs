using ProjectIvy.Model.Binding.Expense;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.View.ExpenseType;
using View = ProjectIvy.Model.View.Expense;

namespace ProjectIvy.BL.Handlers.Expense
{
    public interface IExpenseHandler : IHandler
    {
        void AddFile(string expenseValueId, string fileValueId, ExpenseFileBinding binding);

        int Count(ExpenseGetBinding binding);

        IEnumerable<KeyValuePair<string, int>> CountByDay(ExpenseGetBinding binding);

        IEnumerable<KeyValuePair<string, int>> CountByDayOfWeek(ExpenseGetBinding binding);

        IEnumerable<GroupedByMonth<int>> CountByMonth(ExpenseGetBinding binding);

        IEnumerable<GroupedByYear<int>> CountByYear(ExpenseGetBinding binding);

        PagedView<CountBy<Model.View.Poi.Poi>> CountByPoi(ExpenseGetBinding binding);

        PagedView<CountBy<ExpenseType>> CountByType(ExpenseGetBinding binding);

        PagedView<CountBy<Model.View.Vendor.Vendor>> CountByVendor(ExpenseGetBinding binding);

        int CountTypes(ExpenseGetBinding binding);

        int CountVendors(ExpenseGetBinding binding);

        string Create(ExpenseBinding binding);

        bool Delete(string valueId);

        View.Expense Get(string expenseId);

        PagedView<View.Expense> Get(ExpenseGetBinding binding);

        IEnumerable<View.ExpenseFile> GetFiles(string expenseValueId);

        IEnumerable<GroupedByMonth<decimal>> SumAmountByMonth(ExpenseSumGetBinding binding);

        IEnumerable<GroupedByYear<decimal>> SumAmountByYear(ExpenseSumGetBinding binding);

        Task<IEnumerable<KeyValuePair<string, decimal>>> GetSumByTypeSum(ExpenseSumGetBinding binding);

        Task<decimal> SumAmount(ExpenseSumGetBinding binding);

        bool Update(ExpenseBinding binding);
    }
}
