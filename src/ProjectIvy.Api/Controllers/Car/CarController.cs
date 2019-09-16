using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Car;
using ProjectIvy.Model.Binding.Car;
using ProjectIvy.Model.Constants.Database;
using System;
using System.Collections.Generic;
using View = ProjectIvy.Model.View.Car;

namespace ProjectIvy.Api.Controllers.Car
{
    [Authorize(Roles = UserRole.User)]
    public class CarController : BaseController<CarController>
    {
        private readonly ICarHandler _carHandler;

        public CarController(ILogger<CarController> logger, ICarHandler carHandler) : base(logger)
        {
            _carHandler = carHandler;
        }

        #region Get

        [HttpGet]
        public IEnumerable<View.Car> Get()
        {
            return _carHandler.Get();
        }

        [HttpGet("{carId}/Log/BySession")]
        public IEnumerable<View.CarLogBySession> GetLogBySession(string carId, [FromQuery] CarLogGetBinding binding)
        {
            return _carHandler.GetLogBySession(carId, binding);
        }

        [HttpGet("{carId}/Log/Count")]
        public int GetLogCount(string carId)
        {
            return _carHandler.GetLogCount(carId);
        }

        [HttpGet("{carId}/Log/Latest")]
        public View.CarLog GetLogLatest(string carId, [FromQuery] CarLogGetBinding binding)
        {
            return _carHandler.GetLatestLog(carId, binding);
        }

        [AllowAnonymous]
        [HttpGet("{carId}/Log/Torque.php")]
        public string GetLogTorque(string carId, [FromQuery] CarLogTorqueBinding binding)
        {
            _carHandler.CreateTorqueLog(carId, binding);
            return "OK!";
        }

        #endregion

        #region Post

        [HttpPost("{id}/Log")]
        public DateTime PostLog([FromBody] CarLogBinding binding, string id)
        {
            binding.CarValueId = id;
            return _carHandler.CreateLog(binding);
        }

        #endregion

        #region Put

        [HttpPut("{id}")]
        public IActionResult PutCar(string id, [FromBody] CarBinding car)
        {
            _carHandler.Create(id, car);

            return Ok();
        }

        #endregion
    }
}
