using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Car;
using View = ProjectIvy.Model.View.Car;

namespace ProjectIvy.Api.Controllers.Car;

[Route("[controller]")]
[ApiController]
public class CarModelController : BaseController<CarModelController>
{
    private readonly ICarHandler _carHandler;

    public CarModelController(ILogger<CarModelController> logger, ICarHandler carHandler) : base(logger)
    {
        _carHandler = carHandler;
    }

    [HttpGet("{carModelId}/ServiceInterval")]
    public async Task<IEnumerable<View.CarServiceInterval>> GetServiceIntervals(string carModelId) => await _carHandler.GetServiceIntervals(carModelId);

    [HttpGet("{carModelId}/ServiceType")]
    public async Task<IEnumerable<View.CarServiceType>> GetServiceTypes(string carModelId) => await _carHandler.GetServiceTypes(carModelId);
}
