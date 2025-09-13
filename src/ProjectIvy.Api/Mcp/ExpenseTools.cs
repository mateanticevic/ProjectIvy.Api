using System.ComponentModel;
using System.Threading.Tasks;
using ModelContextProtocol.Server;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using ProjectIvy.Business.Handlers.Expense;
using ProjectIvy.Model.Binding.Expense;

namespace ProjectIvy.Api.Mcp;

[McpServerToolType]
public class ExpenseTools
{
    private readonly IExpenseHandler _expenseHandler;
    private readonly ILogger<ExpenseTools> _logger;

    public ExpenseTools(IExpenseHandler expenseHandler, ILogger<ExpenseTools> logger)
    {
        _expenseHandler = expenseHandler;
        _logger = logger;
    }

    [McpServerTool, Description("Get total sum of expenses")]
    public async Task<decimal> GetSum(DateTime? from, DateTime? to)
    {
        var binding = new ExpenseSumGetBinding()
        {
            From = from,
            To = to ?? DateTime.UtcNow,
        };
        var sw = Stopwatch.StartNew();
        try
        {
            _logger.LogInformation("MCP Tool GetSum start from={From} to={To}", binding.From, binding.To);
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