using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Currency;
using View = ProjectIvy.Model.View.Currency;

namespace ProjectIvy.Api.Controllers.Currency;

[Authorize(ApiScopes.BasicUser)]
public class CurrencyController : BaseController<CurrencyController>
{
    private readonly ICurrencyHandler _currencyHandler;

    public CurrencyController(ILogger<CurrencyController> logger, ICurrencyHandler currencyHandler) : base(logger) => _currencyHandler = currencyHandler;

    [HttpGet]
    public IEnumerable<View.Currency> Get() => _currencyHandler.Get();
}
