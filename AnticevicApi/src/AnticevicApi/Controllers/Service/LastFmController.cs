using AnticevicApi.BL.Services.LastFm;
using AnticevicApi.Common.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace AnticevicApi.Controllers.Service
{
    [Route("service/[controller]")]
    public class LastFmController : BaseController<LastFmController>
    {
        public LastFmController(IOptions<AppSettings> options, ILogger<LastFmController> logger, ILastFmHandler lastFmHandler) : base(options, logger)
        {
            LastFmHandler = lastFmHandler;
        }

        [HttpGet]
        [Route("count")]
        public async Task<int> GetCount()
        {
            return await LastFmHandler.GetTotalCount();
        }
    }
}
