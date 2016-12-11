using AnticevicApi.BL.Handlers.Income;
using AnticevicApi.Config;
using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.View.Income;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class IncomeController : BaseController<IncomeController>
    {
        public IncomeController(IOptions<AppSettings> options, ILogger<IncomeController> logger, IIncomeHandler incomeHandler) : base(options, logger)
        {
            IncomeHandler = incomeHandler;
        }

        #region Get

        [HttpGet]
        public IEnumerable<Income> Get([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int? page = null, [FromQuery] int? pageSize = null)
        {
            return IncomeHandler.Get(new FilteredPagedBinding(from, to, page, pageSize));
        }

        [HttpGet]
        [Route("count")]
        public int GetCount([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            return IncomeHandler.GetCount(from, to);
        }

        [HttpGet]
        [Route("sum")]
        public IEnumerable<AmountInCurrency> Get([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            return IncomeHandler.GetSum(from, to);
        }

        #endregion
    }
}
