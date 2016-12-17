using AnticevicApi.BL.Handlers.Vendor;
using AnticevicApi.Model.Constants;
using AnticevicApi.Model.View.Expense;
using AnticevicApi.Model.View.Vendor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System;
using AnticevicApi.Common.Configuration;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class VendorController : BaseController<VendorController>
    {
        public VendorController(IOptions<AppSettings> options, ILogger<VendorController> logger, IVendorHandler vendorHandler) : base(options, logger)
        {
            VendorHandler = vendorHandler;
        }

        #region Get

        [HttpGet]
        public IEnumerable<Vendor> Get([FromQuery] string contains)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Get), contains);

            return VendorHandler.Get(contains);
        }

        [HttpGet]
        [Route("{valueId}/expense")]
        public IEnumerable<Expense> GetExpenses(string valueId, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetExpenses), valueId, from, to);

            return ExpenseHandler.GetByVendor(valueId, from, to);
        }

        #endregion
    }
}
