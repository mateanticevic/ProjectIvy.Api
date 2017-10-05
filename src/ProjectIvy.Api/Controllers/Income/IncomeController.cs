using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.Income;
using ProjectIvy.Model.Binding.Common;
using ProjectIvy.Model.Binding.Income;
using ProjectIvy.Model.Constants.Database;
using ProjectIvy.Model.View;
using System;
using View = ProjectIvy.Model.View.Income;

namespace ProjectIvy.Api.Controllers.Income
{
    [Authorize(Roles = UserRole.User)]
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
        public PagedView<View.Income> Get([FromQuery] IncomeGetBinding binding)
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
