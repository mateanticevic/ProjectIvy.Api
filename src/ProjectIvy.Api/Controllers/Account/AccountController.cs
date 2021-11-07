using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Account;

namespace ProjectIvy.Api.Controllers.Airport
{
    public class AccountController : BaseController<AccountController>
    {
        private readonly IAccountHandler _accountHandler;

        public AccountController(ILogger<AccountController> logger, IAccountHandler accountHandler) : base(logger)
        {
            _accountHandler = accountHandler;
        }

        [HttpPost("{accountId}/transaction")]
        public async Task<IActionResult> PostTransactions(string accountId)
        {
            var bytes = new byte[HttpContext.Request.ContentLength.Value];

            using (var ms = new MemoryStream(bytes.Length))
            {
                await HttpContext.Request.Body.CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);

                using (var sr = new StreamReader(ms))
                {
                    await _accountHandler.ProcessHacTransactions(accountId, await sr.ReadToEndAsync());
                }
            }

            return Ok();
        }
    }
}
