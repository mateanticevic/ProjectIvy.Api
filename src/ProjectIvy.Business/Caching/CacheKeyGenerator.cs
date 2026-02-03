using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Beer;
using ProjectIvy.Model.Binding.Expense;

namespace ProjectIvy.Business.Caching;

public static class CacheKeyGenerator
{
	private static string GetHash(object obj)
	{
		var json = JsonConvert.SerializeObject(obj);
		using var sha256 = SHA256.Create();
		var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
		return Convert.ToHexString(hashBytes);
	}

	public static string BeerBrandsGet(BrandGetBinding b) => $"{nameof(BeerBrandsGet)}_{GetHash(b)}";

	public static string CityDays(string cityId, FilteredBinding binding) => $"{nameof(CityDays)}_{cityId}_{GetHash(binding)}";

	public static string CurrenciesGet() => nameof(CurrenciesGet);

	public static string CountriesVisited() => nameof(CountriesVisited);

	public static string ExpensesKeys() => nameof(ExpensesKeys);

	public static string ExpensesGet(ExpenseGetBinding b) => $"{nameof(ExpenseGetBinding)}_{GetHash(b)}";

	public static string ExpensesSumAmount(ExpenseSumGetBinding b) => $"{nameof(ExpensesSumAmount)}_{GetHash(b)}";

	public static string LocationDays(string locationId) => $"{nameof(LocationDays)}_{locationId}";

	public static string LocationGeohashes() => nameof(LocationGeohashes);

	public static string TrackingsGetDistance(DateTime? from, DateTime? to) => $"{nameof(TrackingsGetDistance)}_{from}_{to}";

	public static string UserGet() => nameof(UserGet);
}
