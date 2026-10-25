using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Company;
using ProjectIvy.Model.Binding.Company;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Company;

namespace ProjectIvy.Api.Controllers.Company;

[Authorize(ApiScopes.BasicUser)]
public class CompanyController : BaseController<CompanyController>
{
    private readonly ICompanyHandler _companyHandler;

    public CompanyController(ILogger<CompanyController> logger, ICompanyHandler companyHandler) : base(logger)
    {
        _companyHandler = companyHandler;
    }

    [HttpGet]
    public async Task<PagedView<View.Company>> Get([FromQuery] CompanyGetBinding binding) => await _companyHandler.Get(binding);
}
