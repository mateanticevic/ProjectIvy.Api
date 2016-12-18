using AnticevicApi.BL.Handlers.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class TokenController : BaseController<TaskController>
    {
        private readonly ISecurityHandler _securityHandler;

        public TokenController(ILogger<TaskController> logger, ISecurityHandler securityHandler) : base(logger)
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
