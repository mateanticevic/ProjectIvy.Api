using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Salary;
using ProjectIvy.Model.Binding.Salary;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Salary;

namespace ProjectIvy.Api.Controllers.Salary;

[Authorize(ApiScopes.ExpenseUser)]
public class SalaryController : BaseController<SalaryController>
{
    private readonly ISalaryHandler _salaryHandler;

    public SalaryController(ILogger<SalaryController> logger, ISalaryHandler salaryHandler) : base(logger)
    {
        _salaryHandler = salaryHandler;
    }

    [HttpGet]
    public async Task<PagedView<View.Salary>> Get([FromQuery] SalaryGetBinding binding) => await _salaryHandler.Get(binding);

    [HttpGet("{id}")]
    public async Task<View.Salary> Get(string id) => await _salaryHandler.GetSingle(id);

    [HttpPost]
    [ProducesResponseType(typeof(View.Salary), StatusCodes.Status201Created)]
    public async Task<ActionResult<View.Salary>> Post([FromBody] SalaryBinding binding)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var view = await _salaryHandler.Create(binding);
        return CreatedAtAction(nameof(Get), new { id = view.Id }, view);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Put(string id, [FromBody] SalaryBinding binding)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _salaryHandler.Update(id, binding);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(string id)
    {
        await _salaryHandler.Delete(id);
        return NoContent();
    }
}
