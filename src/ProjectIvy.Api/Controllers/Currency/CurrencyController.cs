using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.Currency;
using System.Collections.Generic;
using View = ProjectIvy.Model.View.Currency;

namespace ProjectIvy.Api.Controllers.Currency
{
    [Route("[controller]")]
    public class CurrencyController : BaseController<CurrencyController>
    {
        private readonly ICurrencyHandler _currencyHandler;

        public CurrencyController(ILogger<CurrencyController> logger, ICurrencyHandler currencyHandler) : base(logger) => _currencyHandler = currencyHandler;

        [HttpGet]
        public IEnumerable<View.Currency> Get() => _currencyHandler.Get();
    }
}
