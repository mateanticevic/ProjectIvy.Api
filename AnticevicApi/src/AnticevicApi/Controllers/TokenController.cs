using AnticevicApi.Common.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class TokenController : BaseController<TaskController>
    {
        public TokenController(ILogger<TaskController> logger) : base(logger)
        {

        }

        #region Get

        [HttpPost]
        public string PostToken([FromQuery] string username, [FromQuery] string password)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
