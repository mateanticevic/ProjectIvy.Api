using Microsoft.AspNetCore.Authorization;
using ProjectIvy.Api.Constants;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using ModelContextProtocol.Server;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Expense;
using ProjectIvy.Model.Binding.Expense;
using ProjectIvy.Model.View.ExpenseType;
using ProjectIvy.Model.View;
using ProjectIvy.Business.Handlers.User;
using ProjectIvy.Model.View.Expense;

namespace ProjectIvy.Api.Mcp;

[McpServerToolType]
public class ExpenseTools
{
    private readonly IExpenseHandler _expenseHandler;

    private readonly IExpenseTypeHandler _expenseTypeHandler;

    private readonly ILogger<ExpenseTools> _logger;

    private readonly IUserHandler _userHandler;

    public ExpenseTools(IExpenseHandler expenseHandler,
                        IExpenseTypeHandler expenseTypeHandler,
                        IUserHandler userHandler,
                        ILogger<ExpenseTools> logger)
    {
        _expenseHandler = expenseHandler;
        _expenseTypeHandler = expenseTypeHandler;
        _userHandler = userHandler;
        _logger = logger;
    }

    [Authorize(ApiScopes.ExpenseUser)]
    [McpServerTool(Name = "expense_add"), Description("Add new expense")]
    public async Task<string> AddExpense([Description("Amount of the expense")] decimal amount,
                                         [Description("Expense type id")] string typeId,
                                         [Description("Date when expense occurred, in YYYY-MM-DD format")] DateTime? date = null,
                                         [Description("Currency id in ISO 4217 format")] string currencyId = null)
    {
        var user = await _userHandler.Get();

        string currencyIdFinal = currencyId ?? user.DefaultCurrency.Code;
        var newExpense = new ExpenseBinding()
        {
            Amount = amount,
            CurrencyId = currencyIdFinal,
            Date = date ?? DateTime.UtcNow.Date,
            ExpenseTypeId = typeId,
            PaymentTypeId = "cash"
        };
        var id = await _expenseHandler.Create(newExpense);
        return id;
    }

    [Authorize(ApiScopes.ExpenseUser)]
    [McpServerTool(Name = "expense_get", ReadOnly = true), Description("Get paged list of expenses")]
    public async Task<PagedView<Expense>> GetExpenses([Description("Start date")] DateTime? from,
                                                      [Description("End date")] DateTime? to,
                                                      [Description("Page number")] int page = 1,
                                                      [Description("Page size")] int pageSize = 20,
                                                      [Description("Type ID")] string typeId = null)
    {
        try
        {
            var binding = new ExpenseGetBinding()
            {
                From = from,
                Page = page,
                PageSize = pageSize,
                To = to ?? DateTime.UtcNow,
                TypeId = typeId is null ? null : [typeId],
            };
            return await _expenseHandler.Get(binding);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MCP Tool GetExpenses failed");
            throw;
        }
    }

    [Authorize(ApiScopes.ExpenseUser)]
    [McpServerTool(Name = "expense_types_get", ReadOnly = true), Description("Hierarchy of expense types")]
    public async Task<IEnumerable<Node<ExpenseType>>> GetTypes()
    {
        return await _expenseTypeHandler.GetTree();
    }

    [Authorize(ApiScopes.ExpenseUser)]
    [McpServerTool(Name = "expense_sum", ReadOnly = true, UseStructuredContent = true), Description("Get total sum of expenses or sums grouped by time, currency, or expense type. Returns total or the matching by-group property.")]
    public async Task<ExpenseSum> Sum([Description("Start date for the expense sum calculation")] DateTime? from,
                                   [Description("End date for the expense sum calculation")] DateTime? to,
                                   [Description("Type ID for the expense sum calculation")] string typeId = null,
                                   [Description("Grouping: none (default total), day, dayOfWeek (0 Monday through 6 Sunday), month (calendar months across years, 1-12), monthOfYear (individual year/month periods), year, currency, type, monthOfYearByType, or yearByType. Values are case-insensitive.")] string groupBy = null)
    {
        var binding = new ExpenseSumGetBinding()
        {
            From = from,
            To = to ?? DateTime.UtcNow,
            TypeId = typeId is null ? null : [typeId],
        };
        var sw = Stopwatch.StartNew();
        try
        {
            _logger.LogInformation("MCP Tool GetSum start from={From} to={To} typeId={TypeId} groupBy={GroupBy}", binding.From, binding.To, typeId, groupBy);
            ExpenseSum result = groupBy?.Trim().ToLowerInvariant() switch
            {
                null or "" or "none" => new() { Total = await _expenseHandler.SumAmount(binding) },
                "day" => new() { ByDay = await _expenseHandler.SumAmountByDay(binding) },
                "dayofweek" => new() { ByDayOfWeek = await _expenseHandler.SumAmountByDayOfWeek(binding) },
                "month" => new() { ByMonth = await _expenseHandler.SumAmountByMonth(binding) },
                "monthofyear" => new() { ByMonthOfYear = await _expenseHandler.SumAmountByMonthOfYear(binding) },
                "year" => new() { ByYear = await _expenseHandler.SumAmountByYear(binding) },
                "currency" => new() { ByCurrency = await _expenseHandler.SumByCurrency(binding) },
                "type" => new() { ByType = await _expenseHandler.SumByType(binding) },
                "monthofyearbytype" => new() { ByMonthOfYearByType = await _expenseHandler.SumByMonthOfYearByType(binding) },
                "yearbytype" => new() { ByYearByType = await _expenseHandler.SumByYearByType(binding) },
                _ => throw new ArgumentException("Supported groupBy values: none, day, dayOfWeek, month, monthOfYear, year, currency, type, monthOfYearByType, yearByType.", nameof(groupBy))
            };
            sw.Stop();
            _logger.LogInformation("MCP Tool GetSum success elapsed={Elapsed}ms groupBy={GroupBy} result={Result}", sw.Elapsed.TotalMilliseconds.ToString("F1"), groupBy, result);
            return result;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "MCP Tool GetSum failed elapsed={Elapsed}ms from={From} to={To} groupBy={GroupBy}", sw.Elapsed.TotalMilliseconds.ToString("F1"), binding.From, binding.To, groupBy);
            throw;
        }
    }
}
