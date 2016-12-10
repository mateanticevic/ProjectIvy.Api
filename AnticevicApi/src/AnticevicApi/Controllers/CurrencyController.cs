using AnticevicApi.BL.Handlers;
using AnticevicApi.Config;
using AnticevicApi.Model.View.Currency;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class CurrencyController : BaseController
    {
        public CurrencyController(IOptions<AppSettings> options) : base(options)
        {

        }

        [HttpGet]
        [Route("")]
        public IEnumerable<Currency> Get()
        {
            return CurrencyHandler.Get();
        }
    }
}
