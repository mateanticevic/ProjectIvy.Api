using AnticevicApi.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class TokenController : BaseController<TaskController>
    {
        public TokenController(IOptions<AppSettings> options, ILogger<TaskController> logger) : base(options, logger)
        {

        }

        #region Get

        [HttpPost]
        public string PostToken([FromQuery] string username, [FromQuery] string password)
        {
            return SecurityHandler.IssueToken(username, password);
        }

        #endregion
    }
}
