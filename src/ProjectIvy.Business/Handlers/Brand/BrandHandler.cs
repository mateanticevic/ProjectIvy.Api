using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Brand;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Brand;

namespace ProjectIvy.Business.Handlers.Brand;

public class BrandHandler : Handler<BrandHandler>, IBrandHandler
{
    public BrandHandler(IHandlerContext<BrandHandler> context) : base(context)
    {
    }

    public View.Brand Get(string id)
    {
        using (var context = GetMainContext())
        {
            return context.Brands.SingleOrDefault(x => x.ValueId == id).ConvertTo(x => new View.Brand(x));
        }
    }

    public async Task<PagedView<View.Brand>> Get(BrandGetBinding binding)
    {
        string searchPattern = $"%{binding.Search}%";

        using var context = GetMainContext();
        return await context.Brands.WhereIf(binding?.Search != null,
                                                x => EF.Functions.Like(x.ValueId, searchPattern) ||
                                                     EF.Functions.Like(x.Name, searchPattern))
                                                      .OrderByDescending(x => x.ValueId == binding.Search)
                                                      .ThenBy(x => x.Name)
                                                      .Select(x => new View.Brand(x))
                                                      .ToPagedViewAsync(binding);
    }
}
