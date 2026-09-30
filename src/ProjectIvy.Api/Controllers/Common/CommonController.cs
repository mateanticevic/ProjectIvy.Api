using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Airport;
using ProjectIvy.Business.Handlers.Beer;
using ProjectIvy.Business.Handlers.Currency;
using ProjectIvy.Business.Handlers.Expense;
using ProjectIvy.Business.Handlers.Income;
using ProjectIvy.Business.Handlers.PaymentType;
using ProjectIvy.Business.Handlers.Poi;
using ProjectIvy.Model.Binding.Airline;
using View = ProjectIvy.Model.View;

namespace ProjectIvy.Api.Controllers.Common;

[Authorize(ApiScopes.BasicUser)]
public class CommonController : BaseController<CommonController>
{
    private readonly IAirlineHandler _airlineHandler;

    private readonly IBeerHandler _beerHandler;

    private readonly ICurrencyHandler _currencyHandler;

    private readonly IExpenseTypeHandler _expenseTypeHandler;

    private readonly IIncomeHandler _incomeHandler;

    private readonly IPaymentTypeHandler _paymentHandler;

    private readonly IPoiHandler _poiHandler;

    public CommonController(ILogger<CommonController> logger,
                            ICurrencyHandler currencyHandler,
                            IBeerHandler beerHandler,
                            IPaymentTypeHandler paymentTypeHandler,
                            IPoiHandler poiHandler,
                            IIncomeHandler incomeHandler,
                            IAirlineHandler airlineHandler,
                            IExpenseTypeHandler expenseTypeHandler) : base(logger)
    {
        _beerHandler = beerHandler;
        _currencyHandler = currencyHandler;
        _expenseTypeHandler = expenseTypeHandler;
        _incomeHandler = incomeHandler;
        _paymentHandler = paymentTypeHandler;
        _poiHandler = poiHandler;
        _airlineHandler = airlineHandler;
    }

    [HttpGet("Airline")]
    public async Task<IActionResult> GetAirlines(AirlineGetBinding binding) => Ok(await _airlineHandler.Get(binding));

    [HttpGet("BeerServing")]
    public async Task<IActionResult> GetBeerServings() => Ok(await _beerHandler.GetServings());

    [HttpGet("BeerStyle")]
    public async Task<IActionResult> GetBeerStyles() => Ok(await _beerHandler.GetStyles());

    [HttpGet("Currency")]
    public async Task<IEnumerable<View.Currency.Currency>> GetCurrencies() => await _currencyHandler.Get();

    [HttpGet("ExpenseFileType")]
    public async Task<IEnumerable<View.Expense.ExpenseFileType>> GetExpenseFileTypes() => await _expenseTypeHandler.GetFileTypes();

    [HttpGet("IncomeType")]
    public async Task<IActionResult> GetIncomeTypes() => Ok(await _incomeHandler.GetTypes());

    [HttpGet("PaymentType")]
    public async Task<IActionResult> GetPaymentTypes() => Ok(await _paymentHandler.GetPaymentTypes());

    [HttpGet("PoiCategory")]
    public async Task<IEnumerable<View.Poi.PoiCategory>> GetPoiCategories() => await _poiHandler.GetCategories();
}
