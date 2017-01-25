using AnticevicApi.BL.Handlers.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AnticevicApi.Controllers.Token
{
    [Route("[controller]")]
    public class TokenController : BaseController<TokenController>
    {
        private readonly ISecurityHandler _securityHandler;

        public TokenController(ILogger<TokenController> logger, ISecurityHandler securityHandler) : base(logger)
        {
            _securityHandler = securityHandler;
        }

        #region Get

        [HttpPost]
        public string PostToken([FromQuery] string username, [FromQuery] string password)
        {
            return _securityHandler.CreateToken(username, password);
        }

        #endregion
    }
}
