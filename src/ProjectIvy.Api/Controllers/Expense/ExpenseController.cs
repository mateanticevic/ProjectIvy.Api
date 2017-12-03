﻿using ProjectIvy.BL.Handlers.Expense;
using ProjectIvy.Model.Binding.Common;
using ProjectIvy.Model.Binding.Expense;
using ProjectIvy.Model.Constants.Database;
using ProjectIvy.Model.Constants;
using ProjectIvy.Model.View;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using View = ProjectIvy.Model.View.Expense;

namespace ProjectIvy.Api.Controllers.Expense
{
    [Authorize(Roles = UserRole.User)]
    [Route("[controller]")]
    public class ExpenseController : BaseController<ExpenseController>
    {
        private readonly IExpenseHandler _expenseHandler;

        public ExpenseController(ILogger<ExpenseController> logger, IExpenseHandler expenseHandler) : base(logger)
        {
            _expenseHandler = expenseHandler;
        }

        #region Delete

        [HttpDelete("{valueId}")]
        public bool Delete(string valueId)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Delete), valueId);

            return _expenseHandler.Delete(valueId);
        }

        #endregion

        #region Get

        [HttpGet("{expenseId}")]
        public View.Expense Get(string expenseId) => _expenseHandler.Get(expenseId);

        [HttpGet]
        public PagedView<View.Expense> Get(ExpenseGetBinding binding) => _expenseHandler.Get(binding);

        [HttpGet("Count")]
        public int GetCount([FromQuery] ExpenseGetBinding binding) => _expenseHandler.Count(binding);

        [HttpGet("Count/ByDay")]
        public IEnumerable<KeyValuePair<string, int>> GetCountByDay([FromQuery] ExpenseGetBinding binding) => _expenseHandler.CountByDay(binding);

        [HttpGet("Count/ByMonth")]
        public IEnumerable<GroupedByMonth<int>> GetCountByMonth([FromQuery] ExpenseGetBinding binding) => _expenseHandler.CountByMonth(binding);

        [HttpGet("Count/ByYear")]
        public IEnumerable<GroupedByYear<int>> GetCountByYear([FromQuery] ExpenseGetBinding binding) => _expenseHandler.CountByYear(binding);

        [HttpGet("{expenseId}/File")]
        public IEnumerable<View.ExpenseFile> GetFiles(string expenseId) => _expenseHandler.GetFiles(expenseId);

        [HttpGet("Sum")]
        public async Task<decimal> GetSum([FromQuery] ExpenseSumGetBinding binding) => await _expenseHandler.GetSum(binding);

        [HttpGet("Sum/ByMonth")]
        public async Task<IEnumerable<GroupedByMonth<decimal>>> GetGroupedByMonthSum([FromQuery] ExpenseSumGetBinding binding) => await _expenseHandler.GetSumByMonth(binding);

        [HttpGet("Sum/ByYear")]
        public async Task<IEnumerable<GroupedByYear<decimal>>> GetGroupedByYearSum([FromQuery] ExpenseSumGetBinding binding) => await _expenseHandler.GetSumByYear(binding);

        [HttpGet("Sum/Type")]
        public async Task<IEnumerable<KeyValuePair<string, decimal>>> GetGroupedByTypeSum([FromQuery] ExpenseSumGetBinding binding) => await _expenseHandler.GetSumByTypeSum(binding);

        [HttpGet("Type/Count")]
        public int GetTypesCount([FromQuery] ExpenseGetBinding binding)
        {
            return _expenseHandler.CountTypes(binding);
        }

        [HttpGet("Vendor/Count")]
        public int GetVendorsCount([FromQuery] ExpenseGetBinding binding) => _expenseHandler.CountVendors(binding);

        #endregion

        #region Put

        [HttpPut("{id}")]
        public bool Put(string id, [FromBody] ExpenseBinding binding)
        {
            binding.Id = id;
            return _expenseHandler.Update(binding);
        }

        #endregion

        #region Post

        [HttpPost]
        public string Post([FromBody] ExpenseBinding binding) => _expenseHandler.Create(binding);

        [HttpPost("{expenseId}/File/{fileId}")]
        public IActionResult PostExpenseFile(string expenseId, string fileId, [FromBody] ExpenseFileBinding binding)
        {
            _expenseHandler.AddFile(expenseId, fileId, binding);

            return Ok();
        }

        #endregion
    }
}
