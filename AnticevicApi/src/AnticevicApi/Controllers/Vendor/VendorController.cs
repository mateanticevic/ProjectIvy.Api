using AnticevicApi.BL.Handlers.Expense;
using AnticevicApi.BL.Handlers.Vendor;
using AnticevicApi.Model.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using ViewExpense = AnticevicApi.Model.View.Expense;
using ViewVendor = AnticevicApi.Model.View.Vendor;

namespace AnticevicApi.Controllers.Vendor
{
    [Route("[controller]")]
    public class VendorController : BaseController<VendorController>
    {
        private readonly IExpenseHandler _expenseHandler;
        private readonly IVendorHandler _vendorHandler;

        public VendorController(ILogger<VendorController> logger, IVendorHandler vendorHandler, IExpenseHandler expenseHandler) : base(logger)
        {
            _expenseHandler = expenseHandler;
            _vendorHandler = vendorHandler;
        }

        #region Get

        [HttpGet]
        public IEnumerable<ViewVendor.Vendor> Get([FromQuery] string contains)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Get), contains);

            return _vendorHandler.Get(contains);
        }

        [HttpGet]
        [Route("{valueId}/expense")]
        public IEnumerable<ViewExpense.Expense> GetExpenses(string valueId, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetExpenses), valueId, from, to);

            return _expenseHandler.GetByVendor(valueId, from, to);
        }

        #endregion
    }
}
