using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Constants;
using ProjectIvy.Business.Handlers.Account;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Account;
using ProjectIvy.Model.Binding.Transaction;
using ProjectIvy.Model.View;

namespace ProjectIvy.Api.Controllers.Airport;

public class AccountController : BaseController<AccountController>
{
    private readonly IAccountHandler _accountHandler;

    public AccountController(ILogger<AccountController> logger, IAccountHandler accountHandler) : base(logger)
    {
        _accountHandler = accountHandler;
    }

    [HttpGet]
    public async Task<IEnumerable<Model.View.Account.Account>> Get([FromQuery] AccountGetBinding binding) => await _accountHandler.Get(binding);

    [HttpGet("NetWorth")]
    public async Task<decimal> GetNetWorth() => await _accountHandler.GetNetWorth();

    [HttpGet("{accountId}/overview")]
    public async Task<IActionResult> GetOverview(string accountId) => Ok(await _accountHandler.GetOverview(accountId));

    [HttpGet("{accountId}/transaction")]
    public async Task<PagedView<Model.View.Account.Transaction>> GetTransactions(string accountId, [FromQuery] FilteredPagedBinding binding)
        => await _accountHandler.GetTransactions(accountId, binding);

    [HttpPost("{accountId}/transaction")]
    public async Task<IActionResult> PostTransaction(string accountId, [FromBody] TransactionBinding binding)
    {
        await _accountHandler.CreateTransaction(accountId, binding);
        return Ok();
    }

    [HttpPost("{accountId}/transaction/import")]
    public async Task<IActionResult> PostTransactionImport(string accountId, [FromQuery] TransactionSource transactionSource)
    {
        var bytes = new byte[HttpContext.Request.ContentLength.Value];

        using (var ms = new MemoryStream(bytes.Length))
        {
            await HttpContext.Request.Body.CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);

            using (var sr = new StreamReader(ms))
            {
                switch (transactionSource)
                {
                    case TransactionSource.Hac:
                        await _accountHandler.ProcessHacTransactions(accountId, await sr.ReadToEndAsync());
                        break;
                    case TransactionSource.OtpBank:
                        await _accountHandler.ProcessOtpBankTransactions(accountId, await sr.ReadToEndAsync());
                        break;
                    case TransactionSource.Revolut:
                        await _accountHandler.ProcessRevolutTransactions(accountId, await sr.ReadToEndAsync());
                        break;
                    default:
                        throw new System.Exception();
                }

            }
        }

        return Ok();
    }
}
