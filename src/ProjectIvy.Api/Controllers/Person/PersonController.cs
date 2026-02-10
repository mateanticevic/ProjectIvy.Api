using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Person;
using ProjectIvy.Model.Binding.Person;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Person;

namespace ProjectIvy.Api.Controllers.Person;

[Authorize(ApiScopes.BasicUser)]
public class PersonController : BaseController<PersonController>
{
    private readonly IPersonHandler _personHandler;

    public PersonController(ILogger<PersonController> logger, IPersonHandler personHandler) : base(logger)
    {
        _personHandler = personHandler;
    }

    [HttpGet]
    public async Task<PagedView<View.Person>> Get([FromQuery] PersonGetBinding binding) => await _personHandler.Get(binding);

    [HttpGet("ByDateOfBirth")]
    public async Task<IEnumerable<View.PersonByDateOfBirth>> GetByDateOfBirth() => await _personHandler.GetByDateOfBirth();
}
