using AnticevicApi.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class TokenController : BaseController
    {
        public TokenController(IOptions<AppSettings> options) : base(options)
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
