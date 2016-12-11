using AnticevicApi.BL.Handlers.Currency;
using AnticevicApi.Config;
using AnticevicApi.Model.View.Currency;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class CurrencyController : BaseController<CurrencyController>
    {
        public CurrencyController(IOptions<AppSettings> options, ILogger<CurrencyController> logger, ICurrencyHandler currencyHandler) : base(options, logger)
        {
            CurrencyHandler = currencyHandler;
        }

        [HttpGet]
        [Route("")]
        public IEnumerable<Currency> Get()
        {
            return CurrencyHandler.Get();
        }
    }
}
