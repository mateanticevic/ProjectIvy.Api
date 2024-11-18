using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.WorkDay;
using ProjectIvy.Model.Binding;

namespace ProjectIvy.Api.Controllers;

[Route("[controller]")]
public class WorkDayController : BaseController<WorkDayController>
{
    private readonly IWorkDayHandler _workDayHandler;
    
    public WorkDayController(ILogger<WorkDayController> logger, IWorkDayHandler workDayHandler) : base(logger)
    {
        _workDayHandler = workDayHandler;
    }

    [HttpGet]
    public async Task<IEnumerable<Model.View.WorkDay.WorkDay>> Get([FromQuery] FilteredBinding binding)
        => await _workDayHandler.Get(binding);
}
