using AnticevicApi.BL.Handlers;
using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.Binding.Expense;
using AnticevicApi.Model.View.Expense;
using AnticevicApi.Model.View;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using AnticevicApi.Model.Constants;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class ExpenseController : BaseController
    {
        #region Delete

        [HttpDelete]
        [Route("{valueId}")]
        public bool Delete(string valueId)
        {
            return ExpenseHandler.Delete(valueId);
        }

        #endregion

        #region Get

        [HttpGet]
        [Route("")]
        public PaginatedView<Expense> Get([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery] string expenseTypeValueId, [FromQuery] string vendorValueId)
        {
            return ExpenseHandler.Get(from, to, expenseTypeValueId, vendorValueId, page, pageSize);
        }

        [HttpGet]
        [Route("count")]
        public int GetCount([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            return ExpenseHandler.GetCount(from, to);
        }

        [HttpGet]
        [Route("sum")]
        public decimal GetSum([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            return ExpenseHandler.GetSum(new FilteredBinding(from, to));
        }

        [HttpGet]
        [Route("sum/month")]
        public IEnumerable<KeyValuePair<DateTime, decimal>> GetGroupedByMonthSum([FromQuery] string type)
        {
            return ExpenseHandler.GetGroupedSum(type, TimeGroupingTypes.Month);
        }

        [HttpGet]
        [Route("sum/year")]
        public IEnumerable<KeyValuePair<DateTime, decimal>> GetGroupedByYearSum([FromQuery] string type)
        {
            return ExpenseHandler.GetGroupedSum(type, TimeGroupingTypes.Year);
        }

        [HttpGet]
        [Route("{date:datetime}")]
        public IEnumerable<Expense> GetByDate(DateTime date)
        {
            return ExpenseHandler.GetByDate(date);
        }

        #endregion

        #region Post

        [HttpPost]
        [Route("{valueId}")]
        public bool Put(string valueId, [FromBody] ExpenseBinding binding)
        {
            binding.ValueId = valueId;
            return ExpenseHandler.Update(binding);
        }

        #endregion

        #region Put

        [HttpPut]
        [Route("")]
        public string Put([FromBody] ExpenseBinding binding)
        {
            return ExpenseHandler.Create(binding);
        }

        #endregion
    }
}
