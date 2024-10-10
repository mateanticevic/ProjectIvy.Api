using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Expense;
using ProjectIvy.Model.Binding.Expense;
using ProjectIvy.Model.Binding.File;
using ProjectIvy.Model.Constants;
using ProjectIvy.Model.View;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.Expense;

namespace ProjectIvy.Api.Controllers.Expense;

[Authorize(ApiScopes.ExpenseUser)]
public class ExpenseController : BaseController<ExpenseController>
{
    private readonly IExpenseHandler _expenseHandler;

    public ExpenseController(ILogger<ExpenseController> logger, IExpenseHandler expenseHandler) : base(logger)
    {
        _expenseHandler = expenseHandler;
    }

    [HttpDelete("{valueId}")]
    public bool Delete(string valueId)
    {
        Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Delete), valueId);

        return _expenseHandler.Delete(valueId);
    }

    [HttpGet("{expenseId}")]
    public View.Expense Get(string expenseId)
        => _expenseHandler.Get(expenseId);

    [HttpGet]
    public PagedView<View.Expense> Get(ExpenseGetBinding binding)
        => _expenseHandler.Get(binding);

    [HttpGet("Count")]
    public int GetCount([FromQuery] ExpenseGetBinding binding)
        => _expenseHandler.Count(binding);

    [HttpGet("Count/ByDay")]
    public IEnumerable<KeyValuePair<string, int>> GetCountByDay([FromQuery] ExpenseGetBinding binding)
        => _expenseHandler.CountByDay(binding);

    [HttpGet("Count/ByDayOfWeek")]
    public IEnumerable<KeyValuePair<int, int>> GetCountByDayOfWekk([FromQuery] ExpenseGetBinding binding)
        => _expenseHandler.CountByDayOfWeek(binding);

    [HttpGet("Count/ByMonth")]
    public IActionResult GetCountByMonth([FromQuery] ExpenseGetBinding binding)
        => Ok(_expenseHandler.CountByMonth(binding));

    [HttpGet("Count/ByMonthOfYear")]
    public IEnumerable<KeyValuePair<string, int>> GetCountByMonthOfYear([FromQuery] ExpenseGetBinding binding)
        => _expenseHandler.CountByMonthOfYear(binding);

    [HttpGet("Count/ByYear")]
    public IEnumerable<KeyValuePair<int, int>> GetCountByYear([FromQuery] ExpenseGetBinding binding)
        => _expenseHandler.CountByYear(binding);

    [HttpGet("Count/ByType")]
    public PagedView<KeyValuePair<Model.View.ExpenseType.ExpenseType, int>> GetCountByType([FromQuery] ExpenseGetBinding binding)
        => _expenseHandler.CountByType(binding);

    [HttpGet("Count/ByVendor")]
    public PagedView<KeyValuePair<Model.View.Vendor.Vendor, int>> GetCountByVendor([FromQuery] ExpenseGetBinding binding)
        => _expenseHandler.CountByVendor(binding);

    [HttpGet("{expenseId}/File")]
    public IEnumerable<View.ExpenseFile> GetFiles(string expenseId) =>
        _expenseHandler.GetFiles(expenseId);

    [HttpGet("Sum")]
    public async Task<decimal> GetSum([FromQuery] ExpenseSumGetBinding binding) =>
        await _expenseHandler.SumAmount(binding);

    [HttpGet("Sum/ByCurrency")]
    public async Task<IEnumerable<KeyValuePair<Model.View.Currency.Currency, decimal>>> GetSumByCurrency([FromQuery] ExpenseSumGetBinding binding)
        => await _expenseHandler.SumByCurrency(binding);

    [HttpGet("Sum/ByDay")]
    public async Task<IEnumerable<KeyValuePair<DateTime, decimal>>> GetSumByDay([FromQuery] ExpenseSumGetBinding binding)
        => await _expenseHandler.SumAmountByDay(binding);

    [HttpGet("Sum/ByDayOfWeek")]
    public async Task<IEnumerable<KeyValuePair<int, decimal>>> GetSumByDayOfWeek([FromQuery] ExpenseSumGetBinding binding)
        => await _expenseHandler.SumAmountByDayOfWeek(binding);

    [HttpGet("Sum/ByMonthOfYear")]
    public IEnumerable<KeyValuePair<string, decimal>> GetSumByMonthOfYear([FromQuery] ExpenseSumGetBinding binding)
        => _expenseHandler.SumAmountByMonthOfYear(binding);

    [HttpGet("Sum/ByMonthOfYear/ByType")]
    public async Task<IEnumerable<KeyValuePair<string, IEnumerable<KeyValuePair<string, decimal>>>>> GetSumByMonthOfYearByType([FromQuery] ExpenseSumGetBinding binding)
        => await _expenseHandler.SumByMonthOfYearByType(binding);

    [HttpGet("Sum/ByYear")]
    public IEnumerable<KeyValuePair<int, decimal>> GetSumByYear([FromQuery] ExpenseSumGetBinding binding)
        => _expenseHandler.SumAmountByYear(binding);

    [HttpGet("Sum/ByYear/ByType")]
    public async Task<IEnumerable<KeyValuePair<short, IEnumerable<KeyValuePair<string, decimal>>>>> GetSumByYearByType([FromQuery] ExpenseSumGetBinding binding)
        => await _expenseHandler.SumByYearByType(binding);

    [HttpGet("Sum/ByMonth")]
    public async Task<IEnumerable<KeyValuePair<int, decimal>>> GetSumByMonth([FromQuery] ExpenseSumGetBinding binding)
        => await _expenseHandler.SumAmountByMonth(binding);

    [HttpGet("Sum/ByType")]
    public async Task<IEnumerable<KeyValuePair<string, decimal>>> GetGroupedByTypeSum([FromQuery] ExpenseSumGetBinding binding)
        => await _expenseHandler.SumByType(binding);

    [HttpGet("Type/Count")]
    public int GetTypesCount([FromQuery] ExpenseGetBinding binding)
        => _expenseHandler.CountTypes(binding);

    [HttpGet("Top/Description")]
    public async Task<IEnumerable<string>> GetTopDescriptions([FromQuery] ExpenseGetBinding binding)
        => await _expenseHandler.GetTopDescriptions(binding);

    [HttpGet("Vendor/Count")]
    public int GetVendorsCount([FromQuery] ExpenseGetBinding binding)
        => _expenseHandler.CountVendors(binding);

    [HttpPut("{id}")]
    public bool Put(string id, [FromBody] ExpenseBinding binding)
    {
        binding.Id = id;
        return _expenseHandler.Update(binding);
    }

    [HttpPost]
    public string Post([FromBody] ExpenseBinding binding)
        => _expenseHandler.Create(binding);

    [HttpPost("FromPhoto")]
    public async Task PostFromPhoto()
    {
        var bytes = new byte[HttpContext.Request.ContentLength.Value];

        using (var ms = new System.IO.MemoryStream(bytes.Length))
        {
            await HttpContext.Request.Body.CopyToAsync(ms);
            bytes = ms.ToArray();
            await _expenseHandler.CreateFromPhoto(new FileBinding() { Data = bytes, MimeType = HttpContext.Request.ContentType });
        }
    }

    [HttpPost("{expenseId}/File/{fileId}")]
    public IActionResult PostExpenseFile(string expenseId, string fileId, [FromBody] ExpenseFileBinding binding)
    {
        _expenseHandler.AddFile(expenseId, fileId, binding);

        return Ok();
    }
}