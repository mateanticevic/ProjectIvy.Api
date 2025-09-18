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

namespace ProjectIvy.Api.Mcp;

[McpServerToolType]
public class ExpenseTools
{
    private readonly IExpenseHandler _expenseHandler;
    private readonly IExpenseTypeHandler _expenseTypeHandler;
    private readonly IUserHandler _userHandler;
    private readonly ILogger<ExpenseTools> _logger;

    public ExpenseTools(IExpenseHandler expenseHandler, IExpenseTypeHandler expenseTypeHandler, ILogger<ExpenseTools> logger)
    {
        _expenseHandler = expenseHandler;
        _expenseTypeHandler = expenseTypeHandler;
        _logger = logger;
    }

    [McpServerTool, Description("Add new expense")]
    public string AddExpense([Description("Amount of the expense")] decimal amount,
                                         [Description("Expense type id")] string typeId,
                                         [Description("Date when expense occurred, in YYYY-MM-DD format")] DateTime? date = null,
                                         [Description("Currency id in ISO 4217 format")] string currencyId = null)
    {
        var user = _userHandler.Get();

        string currencyIdFinal = currencyId ?? user.DefaultCurrency.Code;
        var newExpense = new ExpenseBinding()
        {
            Amount = amount,
            CurrencyId = currencyIdFinal,
            Date = date ?? DateTime.UtcNow.Date,
            ExpenseTypeId = typeId,
            PaymentTypeId = "cash"
        };
        var id = _expenseHandler.Create(newExpense);
        return id;
    }

    [McpServerTool, Description("Hierarchy of expense types")]
    public IEnumerable<Node<ExpenseType>> GetTypes()
    {
        return _expenseTypeHandler.GetTree();
    }

    [McpServerTool, Description("Get total sum of expenses")]
    public async Task<decimal> Sum([Description("Start date for the expense sum calculation")] DateTime? from,
                                   [Description("End date for the expense sum calculation")] DateTime? to,
                                   [Description("Type ID for the expense sum calculation")] string typeId = null)
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
            _logger.LogInformation("MCP Tool GetSum start from={From} to={To} typeId={TypeId}", binding.From, binding.To, typeId);
            var x = await _expenseHandler.SumAmount(binding);
            sw.Stop();
            _logger.LogInformation("MCP Tool GetSum success elapsed={Elapsed}ms result={Result}", sw.Elapsed.TotalMilliseconds.ToString("F1"), x);
            return x;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "MCP Tool GetSum failed elapsed={Elapsed}ms from={From} to={To}", sw.Elapsed.TotalMilliseconds.ToString("F1"), binding.From, binding.To);
            throw;
        }
    }
}