using AnticevicApi.BL.Handlers.Income;
using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.View.Income;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class IncomeController : BaseController<IncomeController>
    {
        private IIncomeHandler _incomeHandler;

        public IncomeController(ILogger<IncomeController> logger, IIncomeHandler incomeHandler) : base(logger)
        {
            _incomeHandler = incomeHandler;
        }

        #region Get

        [HttpGet]
        public IEnumerable<Income> Get([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int? page = null, [FromQuery] int? pageSize = null)
        {
            return _incomeHandler.Get(new FilteredPagedBinding(from, to, page, pageSize));
        }

        [HttpGet]
        [Route("count")]
        public int GetCount([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            return _incomeHandler.GetCount(from, to);
        }

        [HttpGet]
        [Route("sum")]
        public IEnumerable<AmountInCurrency> Get([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            return _incomeHandler.GetSum(from, to);
        }

        #endregion
    }
}
