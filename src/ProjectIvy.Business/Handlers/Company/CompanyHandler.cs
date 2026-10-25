using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Model.Binding.Company;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Company;

namespace ProjectIvy.Business.Handlers.Company;

public class CompanyHandler : Handler<CompanyHandler>, ICompanyHandler
{
    public CompanyHandler(IHandlerContext<CompanyHandler> context) : base(context)
    {
    }

    public async Task<PagedView<View.Company>> Get(CompanyGetBinding binding)
    {
        using var context = GetMainContext();
        return await context.Companies.AsNoTracking()
                            .Where(binding)
                            .OrderBy(x => x.Name)
                            .ThenBy(x => x.ValueId)
                            .Select(x => new View.Company(x))
                            .ToPagedViewAsync(binding);
    }
}
