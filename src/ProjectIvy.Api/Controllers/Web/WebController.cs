using ProjectIvy.BL.Handlers.Web;
using ProjectIvy.Model.Binding.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using View = ProjectIvy.Model.View.Web;

namespace ProjectIvy.Api.Controllers.Web
{
    [Route("[controller]")]
    public class WebController : BaseController<WebController>
    {
        private readonly IWebHandler _webHandler;

        public WebController(ILogger<WebController> logger, IWebHandler webHandler) : base(logger)
        {
            _webHandler = webHandler;
        }

        #region Time/

        [HttpGet]
        [Route("time/sum")]
        public IEnumerable<View.WebTime> GetTimeSummed([FromQuery] WebTimeGetPagedBinding binding)
        {
            return _webHandler.GetTimeSummed(binding);
        }

        [HttpGet]
        [Route("time/total")]
        public int GetTimeTotal([FromQuery] WebTimeGetBinding binding)
        {
            return _webHandler.GetTimeSum(binding);
        }

        [HttpGet]
        [Route("time/total/byday")]
        public IEnumerable<View.TimeByDay> GetTimeTotalByDay([FromQuery] WebTimeGetBinding binding)
        {
            return _webHandler.GetTimeTotalByDay(binding);
        }

        #endregion
    }
}
