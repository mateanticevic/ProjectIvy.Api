using System.Linq.Expressions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ProjectIvy.Api.Mcp;
using ProjectIvy.Business.Handlers.Expense;
using ProjectIvy.Business.Handlers.User;
using ProjectIvy.Model.Binding.Expense;
using ProjectIvy.Model.View.Currency;
using ProjectIvy.Model.View.Expense;
using Xunit;

namespace ProjectIvy.Api.EndToEnd.Test;

public sealed class ExpenseSumToolTests
{
    [Theory]
    [InlineData(null, "none")]
    [InlineData("", "none")]
    [InlineData(" \t ", "none")]
    [InlineData(" NoNe ", "none")]
    [InlineData("day", "day")]
    [InlineData("DAYOFWEEK", "dayOfWeek")]
    [InlineData("month", "month")]
    [InlineData(" monthOfYear ", "monthOfYear")]
    [InlineData("year", "year")]
    [InlineData("currency", "currency")]
    [InlineData("type", "type")]
    [InlineData("monthOfYearByType", "monthOfYearByType")]
    [InlineData("yearByType", "yearByType")]
    public async Task DispatchesToSelectedHandlerAndForwardsFilters(string? groupBy, string expectedGrouping)
    {
        var handler = new Mock<IExpenseHandler>(MockBehavior.Strict);
        var expected = ConfigureSum(handler, expectedGrouping);
        var from = new DateTime(2026, 1, 1);
        var to = new DateTime(2026, 10, 15);

        var result = await Tool(handler).Sum(from, to, "food", groupBy!);

        Assert.Equivalent(expected, result);
        var binding = Assert.IsType<ExpenseSumGetBinding>(Assert.Single(handler.Invocations).Arguments[0]);
        Assert.Equal(from, binding.From);
        Assert.Equal(to, binding.To);
        Assert.Equal(new[] { "food" }, binding.TypeId);
        handler.VerifyAll();
        handler.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task OmittedGroupingUsesTotalAndExistingDateDefaults()
    {
        var handler = new Mock<IExpenseHandler>(MockBehavior.Strict);
        ConfigureSum(handler, "none");
        var before = DateTime.UtcNow;

        Assert.Equivalent(new ExpenseSum { Total = 42m }, await Tool(handler).Sum(null, null));

        var binding = Assert.IsType<ExpenseSumGetBinding>(Assert.Single(handler.Invocations).Arguments[0]);
        Assert.Null(binding.From);
        Assert.Null(binding.TypeId);
        Assert.InRange(binding.To!.Value, before, DateTime.UtcNow);
        handler.VerifyAll();
        handler.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InvalidGroupingListsSupportedValuesWithoutCallingHandler()
    {
        var handler = new Mock<IExpenseHandler>(MockBehavior.Strict);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Tool(handler).Sum(null, null, groupBy: "invalid"));

        Assert.Equal("groupBy", exception.ParamName);
        foreach (var value in new[] { "none", "day", "dayOfWeek", "month", "monthOfYear", "year", "currency", "type", "monthOfYearByType", "yearByType" })
            Assert.Contains(value, exception.Message);
        handler.VerifyNoOtherCalls();
    }

    private static ExpenseTools Tool(Mock<IExpenseHandler> handler) => new(handler.Object,
        Mock.Of<IExpenseTypeHandler>(), Mock.Of<IUserHandler>(), NullLogger<ExpenseTools>.Instance);

    internal static ExpenseSum ConfigureSum(Mock<IExpenseHandler> handler, string grouping) => grouping switch
    {
        "none" => new() { Total = Setup(handler, x => x.SumAmount(It.IsAny<ExpenseSumGetBinding>()), 42m) },
        "day" => new() { ByDay = Setup(handler, x => x.SumAmountByDay(It.IsAny<ExpenseSumGetBinding>()),
            new[] { KeyValuePair.Create(new DateTime(2026, 10, 1), 42m) }) },
        "dayOfWeek" => new() { ByDayOfWeek = Setup(handler, x => x.SumAmountByDayOfWeek(It.IsAny<ExpenseSumGetBinding>()),
            new[] { KeyValuePair.Create(0, 42m) }) },
        "month" => new() { ByMonth = Setup(handler, x => x.SumAmountByMonth(It.IsAny<ExpenseSumGetBinding>()),
            new[] { KeyValuePair.Create(10, 42m) }) },
        "monthOfYear" => new() { ByMonthOfYear = Setup(handler, x => x.SumAmountByMonthOfYear(It.IsAny<ExpenseSumGetBinding>()),
            new[] { KeyValuePair.Create("2026-10-1", 42m) }) },
        "year" => new() { ByYear = Setup(handler, x => x.SumAmountByYear(It.IsAny<ExpenseSumGetBinding>()),
            new[] { KeyValuePair.Create(2026, 42m) }) },
        "currency" => new() { ByCurrency = Setup(handler, x => x.SumByCurrency(It.IsAny<ExpenseSumGetBinding>()),
            new[] { KeyValuePair.Create(new Currency { Id = "EUR", Name = "Euro" }, 42m) }) },
        "type" => new() { ByType = Setup(handler, x => x.SumByType(It.IsAny<ExpenseSumGetBinding>()),
            new[] { KeyValuePair.Create("food", 42m) }) },
        "monthOfYearByType" => new() { ByMonthOfYearByType = Setup(handler, x => x.SumByMonthOfYearByType(It.IsAny<ExpenseSumGetBinding>()),
            new[] { KeyValuePair.Create("2026-10-1", Types()) }) },
        "yearByType" => new() { ByYearByType = Setup(handler, x => x.SumByYearByType(It.IsAny<ExpenseSumGetBinding>()),
            new[] { KeyValuePair.Create((short)2026, Types()) }) },
        _ => throw new ArgumentOutOfRangeException(nameof(grouping))
    };

    private static IEnumerable<KeyValuePair<string, decimal>> Types() => [KeyValuePair.Create("food", 42m)];

    private static T Setup<T>(Mock<IExpenseHandler> handler, Expression<Func<IExpenseHandler, Task<T>>> call, T result)
    {
        handler.Setup(call).ReturnsAsync(result);
        return result;
    }
}
