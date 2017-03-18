using AnticevicApi.BL.Handlers.Web;
using AnticevicApi.Model.Binding.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AnticevicApi.Model.Binding.Web;
using System.Collections.Generic;
using System;
using View = AnticevicApi.Model.View.Web;

namespace AnticevicApi.Controllers.Web
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
