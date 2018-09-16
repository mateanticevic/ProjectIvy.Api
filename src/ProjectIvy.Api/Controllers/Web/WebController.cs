using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.Web;
using ProjectIvy.Model.Binding.Web;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using View = ProjectIvy.Model.View.Web;

namespace ProjectIvy.Api.Controllers.Web
{
    [Route("[controller]")]
    public class WebController : BaseController<WebController>
    {
        private readonly IWebHandler _webHandler;

        public WebController(ILogger<WebController> logger, IWebHandler webHandler) : base(logger) => _webHandler = webHandler;

        [HttpGet("Time/Sum")]
        public IEnumerable<View.WebTime> GetTimeSummed([FromQuery] WebTimeGetPagedBinding binding) => _webHandler.GetTimeSummed(binding);

        [HttpGet]
        [Route("Time/Total")]
        public int GetTimeTotal([FromQuery] WebTimeGetBinding binding) => _webHandler.GetTimeSum(binding);

        [HttpGet("Time/Total/ByDay")]
        public IEnumerable<View.TimeByDay> GetTimeTotalByDay([FromQuery] WebTimeGetBinding binding) => _webHandler.GetTimeTotalByDay(binding);

        [HttpGet("Time/Total/ByMonth")]
        public IEnumerable<GroupedByMonth<int>> GetTimeTotalByMonth([FromQuery] WebTimeGetBinding binding) => _webHandler.GetTimeTotalByMonth(binding);

        [HttpGet("Time/Total/ByYear")]
        public IEnumerable<GroupedByYear<int>> GetTimeTotalByYear([FromQuery] WebTimeGetBinding binding) => _webHandler.GetTimeTotalByYear(binding);
    }
}
