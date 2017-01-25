using AnticevicApi.BL.Handlers.Web;
using AnticevicApi.Model.Binding.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        public IEnumerable<View.WebTime> GetTimeSummed([FromQuery] string deviceId,
                                                  [FromQuery] DateTime? from,
                                                  [FromQuery] DateTime? to,
                                                  [FromQuery] int? page = null,
                                                  [FromQuery] int? pageSize = null)
        {
            return _webHandler.GetTimeSummed(new FilteredPagedBinding(from, to, page, pageSize), deviceId);
        }

        [HttpGet]
        [Route("time/total")]
        public int GetTimeTotal([FromQuery] string deviceId,
                                          [FromQuery] DateTime? from,
                                          [FromQuery] DateTime? to)
        {
            return _webHandler.GetTimeTotal(new FilteredBinding(from, to), deviceId);
        }

        [HttpGet]
        [Route("time/total/byday")]
        public IEnumerable<View.TimeByDay> GetTimeTotalByDay([FromQuery] string deviceId,
                                  [FromQuery] DateTime? from,
                                  [FromQuery] DateTime? to)
        {
            return _webHandler.GetTimeTotalByDay(new FilteredBinding(from, to), deviceId);
        }

        #endregion
    }
}
