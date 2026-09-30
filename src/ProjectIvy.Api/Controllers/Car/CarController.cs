using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Car;
using ProjectIvy.Model.Binding.Car;
using View = ProjectIvy.Model.View.Car;

namespace ProjectIvy.Api.Controllers.Car;

public class CarController : BaseController<CarController>
{
    private readonly ICarHandler _carHandler;

    public CarController(ILogger<CarController> logger, ICarHandler carHandler) : base(logger)
    {
        _carHandler = carHandler;
    }

    [HttpGet]
    public async Task<IEnumerable<View.Car>> Get() => await _carHandler.Get();

    [HttpGet("{carId}")]
    public async Task<View.Car> Get(string carId) => await _carHandler.Get(carId);

    [HttpGet("{carId}/Consumption/Avg")]
    public async Task<IActionResult> GetAverageConsumption(string carId) => Ok(await _carHandler.GetAverageConsumption(carId));

    [HttpGet("{carId}/Consumption/ByYear")]
    public async Task<IActionResult> GetAverageConsumptionByYear(string carId) => Ok(await _carHandler.GetAverageConsumptionByYear(carId));

    [HttpGet("{carId}/Fuel")]
    public async Task<IActionResult> GetFuel(string carId) => Ok(await _carHandler.GetFuelings(carId));

    [HttpGet("{carId}/Fuel/Sum/ByMonth")]
    public async Task<IActionResult> GetFuelSumByMonth(string carId) => Ok(await _carHandler.GetFuelByMonth(carId));

    [HttpGet("{carId}/Fuel/Sum/ByYear")]
    public async Task<IActionResult> GetFuelSumByYear(string carId) => Ok(await _carHandler.GetFuelByYear(carId));

    [HttpGet("{carId}/Kilometers/ByYear")]
    public async Task<IEnumerable<KeyValuePair<int, int>>> GetKilometersByYear(string carId) => await _carHandler.GetKilometersByYear(carId);

    [HttpGet("{carId}/Log/BySession")]
    public async Task<IEnumerable<View.CarLogBySession>> GetLogBySession(string carId, [FromQuery] CarLogGetBinding binding) => await _carHandler.GetLogBySession(carId, binding);

    [HttpGet("{carId}/Log/Count")]
    public async Task<int> GetLogCount(string carId) => await _carHandler.GetLogCount(carId);

    [HttpGet("{carId}/Log/Latest")]
    public async Task<View.CarLog> GetLogLatest(string carId, [FromQuery] CarLogGetBinding binding) => await _carHandler.GetLatestLog(carId, binding);

    [HttpGet("{carId}/Log")]
    public async Task<IActionResult> GetLogs(string carId, [FromQuery] CarLogGetBinding binding) => Ok(await _carHandler.GetLogs(carId, binding));

    [AllowAnonymous]
    [HttpGet("{carId}/Log/Torque.php")]
    public async Task<string> GetLogTorque(string carId, [FromQuery] CarLogTorqueBinding binding)
    {
        await _carHandler.CreateTorqueLog(carId, binding);
        return "OK!";
    }

    [HttpPost("{carId}/Fuel")]
    public async Task<IActionResult> PostFuel(string carId, [FromBody] CarFuelingBinding b)
    {
        await _carHandler.NewFueling(carId, b);
        return Ok();
    }

    [HttpPost("{id}/Log")]
    public async Task<DateTime> PostLog([FromBody] CarLogBinding binding, string id)
    {
        binding.CarValueId = id;
        return await _carHandler.CreateLog(binding);
    }

    [HttpPost("{id}/Service")]
    public async Task<IActionResult> PostService(string id, [FromBody] CarServiceBinding binding) => Ok(await _carHandler.CreateService(id, binding));

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCar(string id, [FromBody] CarBinding car)
    {
        await _carHandler.Create(id, car);

        return Ok();
    }
}
