using AnticevicApi.BL.Handlers.Expense;
using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.Binding.Expense;
using AnticevicApi.Model.Constants.Database;
using AnticevicApi.Model.Constants;
using AnticevicApi.Model.View;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using View = AnticevicApi.Model.View.Expense;

namespace AnticevicApi.Controllers.Expense
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
        [Route("{expenseId}")]
        public View.Expense Get(string expenseId)
        {
            return _expenseHandler.Get(expenseId);
        }

        [HttpGet]
        public PagedView<View.Expense> Get(ExpenseGetBinding binding)
        {
            return _expenseHandler.Get(binding);
        }

        [HttpGet]
        [Route("count")]
        public int GetCount([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Delete), from, to);

            return _expenseHandler.Count(new FilteredBinding(from, to));
        }

        [HttpGet]
        [Route("sum")]
        public async Task<decimal> GetSum([FromQuery] ExpenseSumGetBinding binding)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetSum), binding);

            return await _expenseHandler.GetSum(binding);
        }

        [HttpGet]
        [Route("sum/month")]
        public async Task<IEnumerable<GroupedByMonth<decimal>>> GetGroupedByMonthSum([FromQuery] ExpenseSumGetBinding binding)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetGroupedByMonthSum), binding);

            return await _expenseHandler.GetGroupedByMonthSum(binding);
        }

        [HttpGet]
        [Route("sum/year")]
        public async Task<IEnumerable<GroupedByYear<decimal>>> GetGroupedByYearSum([FromQuery] ExpenseSumGetBinding binding)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetGroupedByYearSum), binding);

            return await _expenseHandler.GetGroupedByYearSum(binding);
        }

        [HttpGet]
        [Route("sum/type")]
        public async Task<IEnumerable<KeyValuePair<string, decimal>>> GetGroupedByTypeSum([FromQuery] ExpenseSumGetBinding binding)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetGroupedByTypeSum), binding);

            return await _expenseHandler.GetGroupedByTypeSum(binding);
        }

        #endregion

        #region Post

        [HttpPut]
        [Route("{valueId}")]
        public bool Put(string expenseId, [FromBody] ExpenseBinding binding)
        {
            binding.Id = expenseId;
            return _expenseHandler.Update(binding);
        }

        #endregion

        #region Put

        [HttpPost]
        public string Post([FromBody] ExpenseBinding binding)
        {
            return _expenseHandler.Create(binding);
        }

        #endregion
    }
}
