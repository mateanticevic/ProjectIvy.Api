using AnticevicApi.BL.Handlers.Income;
using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.View;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        public PaginatedView<View.Income> Get([FromQuery] FilteredPagedBinding binding)
        {
            return _incomeHandler.Get(binding);
        }

        [HttpGet]
        [Route("count")]
        public int GetCount([FromQuery] FilteredBinding binding)
        {
            return _incomeHandler.GetCount(binding);
        }

        [HttpGet]
        [Route("sum")]
        public decimal Get([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] string currencyCode)
        {
            return _incomeHandler.GetSum(new FilteredBinding(from, to), currencyCode);
        }

        #endregion
    }
}
