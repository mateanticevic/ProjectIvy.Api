﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Expense;
using ProjectIvy.Model.Binding.Expense;
using ProjectIvy.Model.Constants;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.Expense;

namespace ProjectIvy.Api.Controllers.Expense
{
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
        public View.Expense Get(string expenseId) => _expenseHandler.Get(expenseId);

        [HttpGet]
        public PagedView<View.Expense> Get(ExpenseGetBinding binding) => _expenseHandler.Get(binding);

        [HttpGet("Count")]
        public int GetCount([FromQuery] ExpenseGetBinding binding) => _expenseHandler.Count(binding);

        [HttpGet("Count/ByDay")]
        public IEnumerable<KeyValuePair<string, int>> GetCountByDay([FromQuery] ExpenseGetBinding binding) => _expenseHandler.CountByDay(binding);

        [HttpGet("Count/ByDayOfWeek")]
        public IActionResult GetCountByDayOfWekk([FromQuery] ExpenseGetBinding binding) => Ok(_expenseHandler.CountByDayOfWeek(binding));

        [HttpGet("Count/ByMonth")]
        public IActionResult GetCountByMonth([FromQuery] ExpenseGetBinding binding) => Ok(_expenseHandler.CountByMonth(binding));

        [HttpGet("Count/ByMonthOfYear")]
        public IEnumerable<KeyValuePair<string, int>> GetCountByMonthOfYear([FromQuery] ExpenseGetBinding binding) => _expenseHandler.CountByMonthOfYear(binding);

        [HttpGet("Count/ByYear")]
        public IActionResult GetCountByYear([FromQuery] ExpenseGetBinding binding) => Ok(_expenseHandler.CountByYear(binding));

        [HttpGet("Count/ByType")]
        public IActionResult GetCountByType([FromQuery] ExpenseGetBinding binding) => Ok(_expenseHandler.CountByType(binding));

        [HttpGet("Count/ByVendor")]
        public IActionResult GetCountByVendor([FromQuery] ExpenseGetBinding binding) => Ok(_expenseHandler.CountByVendor(binding));

        [HttpGet("{expenseId}/File")]
        public IEnumerable<View.ExpenseFile> GetFiles(string expenseId) => _expenseHandler.GetFiles(expenseId);

        [HttpGet("Sum")]
        public async Task<IActionResult> GetSum([FromQuery] ExpenseSumGetBinding binding) => Ok(await _expenseHandler.SumAmount(binding));

        [HttpGet("Sum/ByCurrency")]
        public async Task<IActionResult> GetSumByCurrency([FromQuery] ExpenseSumGetBinding binding) => Ok(await _expenseHandler.SumByCurrency(binding));

        [HttpGet("Sum/ByDay")]
        public async Task<IActionResult> GetSumByDay([FromQuery] ExpenseSumGetBinding binding) => Ok(await _expenseHandler.SumAmountByDay(binding));

        [HttpGet("Sum/ByDayOfWeek")]
        public async Task<IActionResult> GetSumByDayOfWeek([FromQuery] ExpenseSumGetBinding binding) => Ok(await _expenseHandler.SumAmountByDayOfWeek(binding));

        [HttpGet("Sum/ByMonthOfYear")]
        public IActionResult GetSumByMonthOfYear([FromQuery] ExpenseSumGetBinding binding) => Ok(_expenseHandler.SumAmountByMonthOfYear(binding));

        [HttpGet("Sum/ByMonthOfYear/ByType")]
        public async Task<IActionResult> GetSumByMonthOfYearByType([FromQuery] ExpenseSumGetBinding binding) => Ok(await _expenseHandler.SumByMonthOfYearByType(binding));

        [HttpGet("Sum/ByYear")]
        public IActionResult GetSumByYear([FromQuery] ExpenseSumGetBinding binding) => Ok(_expenseHandler.SumAmountByYear(binding));

        [HttpGet("Sum/ByYear/ByType")]
        public async Task<IActionResult> GetSumByYearByType([FromQuery] ExpenseSumGetBinding binding) => Ok(await _expenseHandler.SumByYearByType(binding));

        [HttpGet("Sum/ByMonth")]
        public async Task<IActionResult> GetSumByMonth([FromQuery] ExpenseSumGetBinding binding) => Ok(await _expenseHandler.SumAmountByMonth(binding));

        [HttpGet("Sum/ByType")]
        public async Task<IEnumerable<KeyValuePair<string, decimal>>> GetGroupedByTypeSum([FromQuery] ExpenseSumGetBinding binding) => await _expenseHandler.SumByType(binding);

        [HttpGet("Type/Count")]
        public int GetTypesCount([FromQuery] ExpenseGetBinding binding) => _expenseHandler.CountTypes(binding);

        [HttpGet("Top/Description")]
        public async Task<IActionResult> GetTopDescriptions([FromQuery] ExpenseGetBinding binding) => Ok(await _expenseHandler.GetTopDescriptions(binding));

        [HttpGet("Vendor/Count")]
        public int GetVendorsCount([FromQuery] ExpenseGetBinding binding) => _expenseHandler.CountVendors(binding);

        [HttpPut("{id}")]
        public bool Put(string id, [FromBody] ExpenseBinding binding)
        {
            binding.Id = id;
            return _expenseHandler.Update(binding);
        }

        [HttpPost]
        public string Post([FromBody] ExpenseBinding binding) => _expenseHandler.Create(binding);

        [HttpPost("{expenseId}/File/{fileId}")]
        public IActionResult PostExpenseFile(string expenseId, string fileId, [FromBody] ExpenseFileBinding binding)
        {
            _expenseHandler.AddFile(expenseId, fileId, binding);

            return Ok();
        }
    }
}
