using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Vendor;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Vendor;

namespace ProjectIvy.Business.Handlers.Vendor;

public class VendorHandler : Handler<VendorHandler>, IVendorHandler
{
    public VendorHandler(IHandlerContext<VendorHandler> context) : base(context)
    {
    }

    public View.Vendor Get(string id)
    {
        using (var context = GetMainContext())
        {
            return context.Vendors.SingleOrDefault(x => x.ValueId == id).ConvertTo(x => new View.Vendor(x));
        }
    }

    public async Task<PagedView<View.Vendor>> Get(VendorGetBinding binding)
    {
        using var context = GetMainContext();
        var query = context.Vendors.AsQueryable();
        string search = binding?.Search;

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchWords = search.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var searchQuery = query.Where(x => false);

            foreach (var searchWord in searchWords)
            {
                string searchPattern = $"%{searchWord}%";
                searchQuery = searchQuery.Union(query.Where(x => EF.Functions.Like(x.ValueId, searchPattern) ||
                                                                  EF.Functions.Like(x.Name, searchPattern)));
            }

            query = searchQuery;
        }

        return await query.OrderByDescending(x => x.ValueId == search)
                          .ThenBy(x => x.Name)
                          .Select(x => new View.Vendor(x))
                          .ToPagedViewAsync(binding);
    }
}
