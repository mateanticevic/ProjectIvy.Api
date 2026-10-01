using ProjectIvy.Model.Binding.Expense;

namespace ProjectIvy.Data.Test.Extensions;

public class FilteredBindingTests
{
    [Fact]
    public async Task MonthlyOverridesKeepIndependentRangesAndPreserveFilters()
    {
        var binding = new ExpenseSumGetBinding
        {
            From = new DateTime(2026, 1, 1),
            To = new DateTime(2026, 3, 31),
            TargetCurrencyId = "eur",
            TypeId = new[] { "food" },
            ExcludeTypeId = new[] { "rent" },
            ByBaseType = true
        };
        var resume = new TaskCompletionSource<bool>();

        async Task<(DateTime? From, DateTime? To)> ReadAfterAwait(ExpenseSumGetBinding month)
        {
            await resume.Task;
            Assert.Equal(binding.TargetCurrencyId, month.TargetCurrencyId);
            Assert.Equal(binding.TypeId, month.TypeId);
            Assert.Equal(binding.ExcludeTypeId, month.ExcludeTypeId);
            Assert.True(month.ByBaseType);
            return (month.From, month.To);
        }

        var tasks = Enumerable.Range(1, 3).Select(month =>
        {
            var from = new DateTime(2026, month, 1);
            return ReadAfterAwait(binding.OverrideFromTo<ExpenseSumGetBinding>(from, from.AddMonths(1).AddDays(-1)));
        }).ToList();

        resume.SetResult(true);
        var ranges = await Task.WhenAll(tasks);

        for (var month = 1; month <= 3; month++)
        {
            var from = new DateTime(2026, month, 1);
            Assert.Equal(from, ranges[month - 1].From);
            Assert.Equal(from.AddMonths(1).AddDays(-1), ranges[month - 1].To);
        }

        Assert.Equal(new DateTime(2026, 1, 1), binding.From);
        Assert.Equal(new DateTime(2026, 3, 31), binding.To);
    }
}
