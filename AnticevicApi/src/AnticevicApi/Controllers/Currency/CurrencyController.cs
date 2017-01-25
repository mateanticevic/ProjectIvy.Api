using AnticevicApi.BL.Handlers.Currency;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using View = AnticevicApi.Model.View.Currency;

namespace AnticevicApi.Controllers.Currency
{
    [Route("[controller]")]
    public class CurrencyController : BaseController<CurrencyController>
    {
        private readonly ICurrencyHandler _currencyHandler;

        public CurrencyController(ILogger<CurrencyController> logger, ICurrencyHandler currencyHandler) : base(logger)
        {
            _currencyHandler = currencyHandler;
        }

        [HttpGet]
        [Route("")]
        public IEnumerable<View.Currency> Get()
        {
            return _currencyHandler.Get();
        }
    }
}
