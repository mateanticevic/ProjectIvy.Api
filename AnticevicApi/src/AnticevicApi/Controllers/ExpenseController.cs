using AnticevicApi.BL.Handlers.Expense;
using AnticevicApi.Common.Configuration;
using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.Binding.Expense;
using AnticevicApi.Model.Constants;
using AnticevicApi.Model.View.Expense;
using AnticevicApi.Model.View;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class ExpenseController : BaseController<ExpenseController>
    {
        public ExpenseController(IOptions<AppSettings> options, ILogger<ExpenseController> logger, IExpenseHandler expenseHandler) : base(options, logger)
        {
            ExpenseHandler = expenseHandler;
        }

        #region Delete

        [HttpDelete]
        [Route("{valueId}")]
        public bool Delete(string valueId)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Delete), valueId);

            return ExpenseHandler.Delete(valueId);
        }

        #endregion

        #region Get

        [HttpGet]
        [Route("")]
        public PaginatedView<Expense> Get([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery] string expenseTypeValueId, [FromQuery] string vendorValueId)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Get), from, to, page, pageSize, expenseTypeValueId, vendorValueId);

            return ExpenseHandler.Get(from, to, expenseTypeValueId, vendorValueId, page, pageSize);
        }

        [HttpGet]
        [Route("count")]
        public int GetCount([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Delete), from, to);

            return ExpenseHandler.GetCount(new FilteredBinding(from, to));
        }

        [HttpGet]
        [Route("sum")]
        public decimal GetSum([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetSum), from, to);

            return ExpenseHandler.GetSum(new FilteredBinding(from, to));
        }

        [HttpGet]
        [Route("sum/month")]
        public IEnumerable<KeyValuePair<DateTime, decimal>> GetGroupedByMonthSum([FromQuery] string type)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetGroupedByMonthSum), type);

            return ExpenseHandler.GetGroupedSum(type, TimeGroupingTypes.Month);
        }

        [HttpGet]
        [Route("sum/year")]
        public IEnumerable<KeyValuePair<DateTime, decimal>> GetGroupedByYearSum([FromQuery] string type)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetGroupedByYearSum), type);

            return ExpenseHandler.GetGroupedSum(type, TimeGroupingTypes.Year);
        }

        [HttpGet]
        [Route("{date:datetime}")]
        public IEnumerable<Expense> GetByDate(DateTime date)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetByDate), date);

            return ExpenseHandler.GetByDate(date);
        }

        #endregion

        #region Post

        [HttpPost]
        [Route("{valueId}")]
        public bool Put(string valueId, [FromBody] ExpenseBinding binding)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Put), binding);

            binding.ValueId = valueId;
            return ExpenseHandler.Update(binding);
        }

        #endregion

        #region Put

        [HttpPut]
        [Route("")]
        public string Put([FromBody] ExpenseBinding binding)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Put), binding);

            return ExpenseHandler.Create(binding);
        }

        #endregion
    }
}
