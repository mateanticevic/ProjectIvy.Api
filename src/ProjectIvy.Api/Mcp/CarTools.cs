using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Server;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Car;
using ProjectIvy.Model.Binding.Car;

namespace ProjectIvy.Api.Mcp;

[McpServerToolType]
public class CarTools
{
    private readonly ICarHandler _carHandler;

    public CarTools(ICarHandler carHandler)
    {
        _carHandler = carHandler;
    }

    [Authorize(ApiScopes.BasicUser)]
    [McpServerTool(Name = "create_car_log")]
    [Description("Creates a car odometer log and returns its timestamp. The odometer reading must be at least the latest recorded reading.")]
    public Task<DateTime> CreateCarLog(
        [Description("Public car ID.")] string carValueId,
        [Description("Odometer reading in kilometers.")] int odometer)
        => _carHandler.CreateLog(carValueId, new CarLogBinding { Odometer = odometer });
}
