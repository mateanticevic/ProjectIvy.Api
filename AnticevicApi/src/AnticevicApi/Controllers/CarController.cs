using AnticevicApi.BL.Handlers.Car;
using AnticevicApi.Common.Configuration;
using AnticevicApi.Model.Binding.Car;
using AnticevicApi.Model.View.Car;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class CarController : BaseController<CarController>
    {
        public CarController(IOptions<AppSettings> options, ILogger<CarController> logger, ICarHandler carHandler) : base(options, logger)
        {
            CarHandler = carHandler;
        }

        #region Get

        [HttpGet]
        [Route("{valueId}/log/count")]
        public int GetLogCount(string valueId)
        {
            return CarHandler.GetLogCount(valueId);
        }

        [HttpGet]
        [Route("{valueId}/log/latest")]
        public CarLog GetLogLatest(string valueId)
        {
            return CarHandler.GetLatestLog(valueId);
        }

        #endregion

        #region Put

        [HttpPut]
        [Route("{valueId}/log")]
        public DateTime PutLog([FromBody] CarLogBinding binding, string valueId)
        {
            binding.CarValueId = valueId;
            return CarHandler.CreateLog(binding);
        }

        #endregion
    }
}
