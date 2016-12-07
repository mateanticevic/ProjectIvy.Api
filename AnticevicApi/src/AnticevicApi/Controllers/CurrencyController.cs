using AnticevicApi.BL.Handlers;
using AnticevicApi.Model.View.Currency;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class CurrencyController : BaseController
    {
        [HttpGet]
        [Route("")]
        public IEnumerable<Currency> Get()
        {
            return CurrencyHandler.Get();
        }
    }
}
