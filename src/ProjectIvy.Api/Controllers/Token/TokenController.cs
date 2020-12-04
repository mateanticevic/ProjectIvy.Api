using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Security;
using ProjectIvy.Model.View;

namespace ProjectIvy.Api.Controllers.Token
{
    public class TokenController : BaseController<TokenController>
    {
        private readonly ISecurityHandler _securityHandler;

        public TokenController(ILogger<TokenController> logger, ISecurityHandler securityHandler) : base(logger) => _securityHandler = securityHandler;

        [HttpPost]
        public ActionResult PostToken([FromQuery] string username, [FromQuery] string password)
        {
            var requestContext = new RequestContext();

            try
            {
                string userAgentHeader = Request.Headers["User-Agent"].ToString();
                requestContext.UserAgent = UAParser.Parser.GetDefault().ParseUserAgent(userAgentHeader).ToString();
                requestContext.OperatingSystem = UAParser.Parser.GetDefault().ParseOS(userAgentHeader).ToString();
                requestContext.IpAddress = Request.Headers["X-Forwarded-For"].ToString() ?? HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
            catch
            {
            }

            string token = _securityHandler.CreateToken(username, password, requestContext);
            return new JsonResult(token);
        }
    }
}
