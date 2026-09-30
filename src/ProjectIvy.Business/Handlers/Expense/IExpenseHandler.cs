using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Expense;
using ProjectIvy.Model.Binding.File;
using ProjectIvy.Model.View;
using ProjectIvy.Model.View.ExpenseType;
using View = ProjectIvy.Model.View.Expense;

namespace ProjectIvy.Business.Handlers.Expense;

public interface IExpenseHandler : IHandler
{
    Task AddFile(string expenseValueId, string fileValueId, ExpenseFileBinding binding);

    Task<int> Count(ExpenseGetBinding binding);

    Task<IEnumerable<KeyValuePair<string, int>>> CountByDay(ExpenseGetBinding binding);

    Task<IEnumerable<KeyValuePair<int, int>>> CountByDayOfWeek(ExpenseGetBinding binding);

    Task<IEnumerable<KeyValuePair<int, int>>> CountByMonth(ExpenseGetBinding binding);

    Task<IEnumerable<KeyValuePair<string, int>>> CountByMonthOfYear(ExpenseGetBinding binding);

    Task<PagedView<KeyValuePair<ExpenseType, int>>> CountByType(ExpenseGetBinding binding);

    Task<PagedView<KeyValuePair<Model.View.Vendor.Vendor, int>>> CountByVendor(ExpenseGetBinding binding);

    Task<IEnumerable<KeyValuePair<int, int>>> CountByYear(ExpenseGetBinding binding);

    Task<int> CountTypes(ExpenseGetBinding binding);

    Task<int> CountVendors(ExpenseGetBinding binding);

    Task<string> Create(ExpenseBinding binding);

    Task CreateFromFile(FileBinding binding);

    Task Delete(string valueId);

    Task<View.Expense> Get(string expenseId);

    Task<PagedView<View.Expense>> Get(ExpenseGetBinding binding);

    Task<IEnumerable<View.ExpenseFile>> GetFiles(string expenseValueId);

    Task<IEnumerable<string>> GetTopDescriptions(ExpenseGetBinding binding);

    Task<decimal> SumAmount(ExpenseSumGetBinding binding);

    Task<IEnumerable<KeyValuePair<DateTime, decimal>>> SumAmountByDay(ExpenseSumGetBinding binding);

    Task<IEnumerable<KeyValuePair<int, decimal>>> SumAmountByDayOfWeek(ExpenseSumGetBinding binding);

    Task<IEnumerable<KeyValuePair<int, decimal>>> SumAmountByMonth(ExpenseSumGetBinding binding);

    Task<IEnumerable<KeyValuePair<string, decimal>>> SumAmountByMonthOfYear(ExpenseSumGetBinding binding);

    Task<IEnumerable<KeyValuePair<int, decimal>>> SumAmountByYear(ExpenseSumGetBinding binding);

    Task<IEnumerable<KeyValuePair<Model.View.Currency.Currency, decimal>>> SumByCurrency(ExpenseSumGetBinding binding);

    Task<IEnumerable<KeyValuePair<string, IEnumerable<KeyValuePair<string, decimal>>>>> SumByMonthOfYearByType(ExpenseSumGetBinding binding);

    Task<IEnumerable<KeyValuePair<string, decimal>>> SumByType(ExpenseSumGetBinding binding);

    Task<IEnumerable<KeyValuePair<short, IEnumerable<KeyValuePair<string, decimal>>>>> SumByYearByType(ExpenseSumGetBinding binding);

    Task<bool> Update(ExpenseBinding binding);
}
