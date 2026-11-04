using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Account;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.View;
using ProjectIvy.Model.View.Account;

namespace ProjectIvy.Api.Controllers.Transaction;

public class TransactionController : BaseController<TransactionController>
{
    private readonly IAccountHandler _accountHandler;

    public TransactionController(ILogger<TransactionController> logger, IAccountHandler accountHandler) : base(logger)
    {
        _accountHandler = accountHandler;
    }

    [HttpGet]
    public Task<PagedView<TransactionWithAccount>> Get([FromQuery] FilteredPagedBinding binding)
        => _accountHandler.GetTransactions(binding);
}
