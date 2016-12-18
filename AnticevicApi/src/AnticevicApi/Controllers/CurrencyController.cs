using AnticevicApi.BL.Handlers.Currency;
using AnticevicApi.Model.View.Currency;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace AnticevicApi.Controllers
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
        public IEnumerable<Currency> Get()
        {
            return _currencyHandler.Get();
        }
    }
}
