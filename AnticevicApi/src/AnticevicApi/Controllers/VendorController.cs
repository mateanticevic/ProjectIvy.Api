using AnticevicApi.BL.Handlers;
using AnticevicApi.Config;
using AnticevicApi.Model.View.Expense;
using AnticevicApi.Model.View.Vendor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class VendorController : BaseController
    {
        public VendorController(IOptions<AppSettings> options) : base(options)
        {

        }

        #region Get

        [HttpGet]
        public IEnumerable<Vendor> Get([FromQuery] string contains)
        {
            return VendorHandler.Get(contains);
        }

        [HttpGet]
        [Route("{valueId}/expense")]
        public IEnumerable<Expense> GetExpenses(string valueId, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            return ExpenseHandler.GetByVendor(valueId, from, to);
        }

        #endregion
    }
}
