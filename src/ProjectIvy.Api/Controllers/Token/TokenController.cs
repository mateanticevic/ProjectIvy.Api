using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Security;

namespace ProjectIvy.Api.Controllers.Token
{
    public class TokenController : BaseController<TokenController>
    {
        private readonly ISecurityHandler _securityHandler;

        public TokenController(ILogger<TokenController> logger, ISecurityHandler securityHandler) : base(logger) => _securityHandler = securityHandler;

        [HttpPost]
        public ActionResult PostToken([FromQuery] string username, [FromQuery] string password)
        {
            string token = _securityHandler.CreateToken(username, password);

            return new JsonResult(token);
        }
    }
}
