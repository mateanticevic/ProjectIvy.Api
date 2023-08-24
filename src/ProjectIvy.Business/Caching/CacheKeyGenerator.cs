using Newtonsoft.Json;
using ProjectIvy.Model.Binding.Expense;

namespace ProjectIvy.Business.Caching
{
    public static class CacheKeyGenerator
	{
		public static string CurrenciesGet() => nameof(CurrenciesGet);

		public static string ExpensesKeys() => nameof(ExpensesKeys);

		public static string ExpensesGet(ExpenseGetBinding b) => $"{nameof(ExpenseGetBinding)}_{JsonConvert.SerializeObject(b)}"; 

		public static string ExpensesSumAmount(ExpenseSumGetBinding b) => $"{nameof(ExpensesSumAmount)}_{JsonConvert.SerializeObject(b)}";

		public static string TrackingsGetDistance(DateTime? from, DateTime? to) => $"{nameof(TrackingsGetDistance)}_{from}_{to}";
	}
}

