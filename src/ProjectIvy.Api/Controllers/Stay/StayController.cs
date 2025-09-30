using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Stay;
using ProjectIvy.Model.Binding.Stay;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Stay;

namespace ProjectIvy.Api.Controllers.Stay;

public class StayController : BaseController<StayController>
{
    private readonly IStayHandler _stayHandler;

    public StayController(ILogger<StayController> logger, IStayHandler stayHandler) : base(logger) => _stayHandler = stayHandler;

    [HttpGet]
    public async Task<PagedView<View.Stay>> Get(StayGetBinding binding) => await _stayHandler.GetStays(binding);

    [HttpPost("{date}")]
    public async Task<StatusCodeResult> Post(DateTime date, [FromBody] StayBinding binding)
    {
        binding.Date = date;
        await _stayHandler.AddStay(binding);

        return new StatusCodeResult(StatusCodes.Status201Created);
    }
}