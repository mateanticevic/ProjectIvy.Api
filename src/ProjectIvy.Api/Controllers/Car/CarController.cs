using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.Car;
using ProjectIvy.Model.Binding.Car;
using ProjectIvy.Model.Constants.Database;
using System.Collections.Generic;
using System;
using View = ProjectIvy.Model.View.Car;

namespace ProjectIvy.Api.Controllers.Car
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
        public IEnumerable<View.Car> Get()
        {
            return _carHandler.Get();
        }

        [HttpGet("{id}/Log/Count")]
        public int GetLogCount(string id)
        {
            return _carHandler.GetLogCount(id);
        }

        [HttpGet("{id}/Log/Latest")]
        public View.CarLog GetLogLatest(string id, [FromQuery] CarLogGetBinding binding)
        {
            return _carHandler.GetLatestLog(id, binding);
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
