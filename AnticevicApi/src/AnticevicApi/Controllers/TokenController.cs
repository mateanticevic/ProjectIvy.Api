using Microsoft.AspNetCore.Mvc;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class TokenController : BaseController
    {
        [HttpPost]
        public string PostToken([FromQuery] string username, [FromQuery] string password)
        {
            return SecurityHandler.IssueToken(username, password);
        }
    }
}
