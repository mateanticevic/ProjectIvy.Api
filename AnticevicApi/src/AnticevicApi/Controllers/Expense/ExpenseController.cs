using AnticevicApi.BL.Handlers.Expense;
using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.Binding.Expense;
using AnticevicApi.Model.Constants;
using AnticevicApi.Model.View;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using View = AnticevicApi.Model.View.Expense;

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
        public PaginatedView<View.Expense> Get([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery] string expenseTypeValueId, [FromQuery] string vendorValueId)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Get), from, to, page, pageSize, expenseTypeValueId, vendorValueId);

            return _expenseHandler.Get(from, to, expenseTypeValueId, vendorValueId, page, pageSize);
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
        public decimal GetSum([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] string currencyCode)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetSum), from, to);

            return _expenseHandler.GetSum(new FilteredBinding(from, to), currencyCode);
        }

        [HttpGet]
        [Route("sum/month")]
        public IEnumerable<KeyValuePair<DateTime, decimal>> GetGroupedByMonthSum([FromQuery] string type)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetGroupedByMonthSum), type);

            return _expenseHandler.GetGroupedSum(type, TimeGroupingTypes.Month);
        }

        [HttpGet]
        [Route("sum/year")]
        public IEnumerable<KeyValuePair<DateTime, decimal>> GetGroupedByYearSum([FromQuery] string type)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetGroupedByYearSum), type);

            return _expenseHandler.GetGroupedSum(type, TimeGroupingTypes.Year);
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
