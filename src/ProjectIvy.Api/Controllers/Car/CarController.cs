using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Car;
using ProjectIvy.Model.Binding.Car;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.Car;

namespace ProjectIvy.Api.Controllers.Car
{
    public class CarController : BaseController<CarController>
    {
        private readonly ICarHandler _carHandler;

        public CarController(ILogger<CarController> logger, ICarHandler carHandler) : base(logger)
        {
            _carHandler = carHandler;
        }

        [HttpGet]
        public IEnumerable<View.Car> Get() => _carHandler.Get();

        [HttpGet("{carId}")]
        public View.Car Get(string carId) => _carHandler.Get(carId);

        [HttpGet("{carId}/Consumption/Avg")]
        public async Task<IActionResult> GetAverageConsumption(string carId) => Ok(await _carHandler.GetAverageConsumption(carId));

        [HttpGet("{carId}/Consumption/ByYear")]
        public async Task<IActionResult> GetAverageConsumptionByYear(string carId) => Ok(await _carHandler.GetAverageConsumptionByYear(carId));

        [HttpGet("{carId}/Fuel")]
        public async Task<IActionResult> GetFuel(string carId) => Ok(await _carHandler.GetFuelings(carId));

        [HttpGet("{carId}/Fuel/Sum/ByMonth")]
        public IActionResult GetFuelSumByMonth(string carId) => Ok(_carHandler.GetFuelByMonth(carId));

        [HttpGet("{carId}/Fuel/Sum/ByYear")]
        public IActionResult GetFuelSumByYear(string carId) => Ok(_carHandler.GetFuelByYear(carId));

        [HttpGet("{carId}/Kilometers/ByYear")]
        public async Task<IEnumerable<KeyValuePair<int, int>>> GetKilometersByYear(string carId) => await _carHandler.GetKilometersByYear(carId);

        [HttpPost("{carId}/Fuel")]
        public async Task<IActionResult> PostFuel(string carId, [FromBody] CarFuelingBinding b)
        {
            await _carHandler.NewFueling(carId, b);
            return Ok();
        }

        [HttpGet("{carId}/Log/BySession")]
        public IEnumerable<View.CarLogBySession> GetLogBySession(string carId, [FromQuery] CarLogGetBinding binding) => _carHandler.GetLogBySession(carId, binding);

        [HttpGet("{carId}/Log/Count")]
        public int GetLogCount(string carId) => _carHandler.GetLogCount(carId);

        [HttpGet("{carId}/Log/Latest")]
        public View.CarLog GetLogLatest(string carId, [FromQuery] CarLogGetBinding binding) => _carHandler.GetLatestLog(carId, binding);

        [HttpGet("{carId}/Log")]
        public async Task<IActionResult> GetLogs(string carId, [FromQuery] CarLogGetBinding binding) => Ok(await _carHandler.GetLogs(carId, binding));

        [AllowAnonymous]
        [HttpGet("{carId}/Log/Torque.php")]
        public string GetLogTorque(string carId, [FromQuery] CarLogTorqueBinding binding)
        {
            _carHandler.CreateTorqueLog(carId, binding);
            return "OK!";
        }

        [HttpPost("{id}/Log")]
        public DateTime PostLog([FromBody] CarLogBinding binding, string id)
        {
            binding.CarValueId = id;
            return _carHandler.CreateLog(binding);
        }

        [HttpPost("{id}/Service")]
        public async Task<IActionResult> PostService(string id, [FromBody] CarServiceBinding binding) => Ok(await _carHandler.CreateService(id, binding));

        [HttpPut("{id}")]
        public IActionResult PutCar(string id, [FromBody] CarBinding car)
        {
            _carHandler.Create(id, car);

            return Ok();
        }
    }
}
