using Newtonsoft.Json;
using ProjectIvy.Model.Binding.Beer;
using ProjectIvy.Model.Binding.Expense;

namespace ProjectIvy.Business.Caching
{
    public static class CacheKeyGenerator
	{
		public static string BeerBrandsGet(BrandGetBinding b) => $"{nameof(BeerBrandsGet)}_{JsonConvert.SerializeObject(b)}";

		public static string CityDays(string cityId) => $"{nameof(CityDays)}_{cityId}";

		public static string CurrenciesGet() => nameof(CurrenciesGet);

		public static string CountriesVisited() => nameof(CountriesVisited);

        public static string ExpensesKeys() => nameof(ExpensesKeys);

		public static string ExpensesGet(ExpenseGetBinding b) => $"{nameof(ExpenseGetBinding)}_{JsonConvert.SerializeObject(b)}"; 

		public static string ExpensesSumAmount(ExpenseSumGetBinding b) => $"{nameof(ExpensesSumAmount)}_{JsonConvert.SerializeObject(b)}";

		public static string LocationDays(string locationId) => $"{nameof(LocationDays)}_{locationId}";

		public static string LocationGeohashes() => nameof(LocationGeohashes);

		public static string TrackingsGetDistance(DateTime? from, DateTime? to) => $"{nameof(TrackingsGetDistance)}_{from}_{to}";

		public static string UserGet() => nameof(UserGet);
	}
}

