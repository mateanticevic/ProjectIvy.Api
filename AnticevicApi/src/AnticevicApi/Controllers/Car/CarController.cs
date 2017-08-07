using AnticevicApi.BL.Handlers.Car;
using AnticevicApi.Model.Binding.Car;
using AnticevicApi.Model.Constants.Database;
using AnticevicApi.Model.View.Car;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace AnticevicApi.Controllers.Car
{
    [Authorize(Roles = UserRole.User)]
    [Route("[controller]")]
    public class CarController : BaseController<CarController>
    {
        private readonly ICarHandler _carHandler;

        public CarController(ILogger<CarController> logger, ICarHandler carHandler) : base(logger)
        {
            _carHandler = carHandler;
        }

        #region Get

        [HttpGet]
        [Route("{valueId}/log/count")]
        public int GetLogCount(string valueId)
        {
            return _carHandler.GetLogCount(valueId);
        }

        [HttpGet]
        [Route("{valueId}/log/latest")]
        public CarLog GetLogLatest(string valueId)
        {
            return _carHandler.GetLatestLog(valueId);
        }

        #endregion

        #region Post

        [HttpPost]
        [Route("{valueId}/log")]
        public DateTime PostLog([FromBody] CarLogBinding binding, string valueId)
        {
            binding.CarValueId = valueId;
            return _carHandler.CreateLog(binding);
        }

        #endregion
    }
}
