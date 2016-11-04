using AnticevicApi.Model.View.Expense;
using AnticevicApi.Model.View.ExpenseType;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class ExpenseTypeController : BaseController
    {
        #region Get

        [HttpGet]
        public IEnumerable<ExpenseType> Get()
        {
            return ExpenseTypeHandler.Get();
        }

        [HttpGet]
        [Route("{valueId}/expense")]
        public IEnumerable<Expense> GetExpenses(string valueId, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            return ExpenseHandler.GetByType(valueId, from, to);
        }

        #endregion
    }
}
