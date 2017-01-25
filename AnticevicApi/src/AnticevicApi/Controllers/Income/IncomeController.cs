using AnticevicApi.BL.Handlers.Income;
using AnticevicApi.Model.Binding.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using View = AnticevicApi.Model.View.Income;

namespace AnticevicApi.Controllers.Income
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
        public IEnumerable<View.Income> Get([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int? page = null, [FromQuery] int? pageSize = null)
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
        public IEnumerable<View.AmountInCurrency> Get([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            return _incomeHandler.GetSum(from, to);
        }

        #endregion
    }
}
