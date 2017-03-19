﻿using AnticevicApi.BL.Handlers.Expense;
using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.Binding.Expense;
using AnticevicApi.Model.Constants;
using AnticevicApi.Model.View;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using View = AnticevicApi.Model.View.Expense;
using System.Threading.Tasks;

namespace AnticevicApi.Controllers.Expense
{
    [Route("[controller]")]
    public class ExpenseController : BaseController<ExpenseController>
    {
        private readonly IExpenseHandler _expenseHandler;

        public ExpenseController(ILogger<ExpenseController> logger, IExpenseHandler expenseHandler) : base(logger)
        {
            _expenseHandler = expenseHandler;
        }

        #region Delete

        [HttpDelete]
        [Route("{valueId}")]
        public bool Delete(string valueId)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Delete), valueId);

            return _expenseHandler.Delete(valueId);
        }

        #endregion

        #region Get

        [HttpGet]
        [Route("")]
        public PaginatedView<View.Expense> Get(ExpenseGetBinding binding)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Get), binding);

            return _expenseHandler.Get(binding);
        }

        [HttpGet]
        [Route("count")]
        public int GetCount([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Delete), from, to);

            return _expenseHandler.GetCount(new FilteredBinding(from, to));
        }

        [HttpGet]
        [Route("sum")]
        public async Task<decimal> GetSum([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] string currencyCode)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetSum), from, to);

            return await _expenseHandler.GetSum(new FilteredBinding(from, to), currencyCode);
        }

        [HttpGet]
        [Route("sum/month")]
        public async Task<IEnumerable<GroupedByMonth<decimal>>> GetGroupedByMonthSum([FromQuery] string currencyId)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetGroupedByMonthSum), currencyId);

            return await _expenseHandler.GetGroupedByMonthSum(currencyId);
        }

        [HttpGet]
        [Route("sum/year")]
        public async Task<IEnumerable<GroupedByYear<decimal>>> GetGroupedByYearSum([FromQuery] string currencyId)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetGroupedByYearSum), currencyId);

            return await _expenseHandler.GetGroupedByYearSum(currencyId);
        }

        [HttpGet]
        [Route("{date:datetime}")]
        public IEnumerable<View.Expense> GetByDate(DateTime date)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetByDate), date);

            return _expenseHandler.GetByDate(date);
        }

        #endregion

        #region Post

        [HttpPost]
        [Route("{valueId}")]
        public bool Put(string valueId, [FromBody] ExpenseBinding binding)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Put), binding);

            binding.ValueId = valueId;
            return _expenseHandler.Update(binding);
        }

        #endregion

        #region Put

        [HttpPut]
        [Route("")]
        public string Put([FromBody] ExpenseBinding binding)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Put), binding);

            return _expenseHandler.Create(binding);
        }

        #endregion
    }
}
