using System.Linq;
using ProjectIvy.Model.Binding.Company;
using ProjectIvy.Model.Database.Main.Common;

namespace ProjectIvy.Data.Extensions.Entities;

public static class CompanyExtensions
{
    public static IQueryable<Company> Where(this IQueryable<Company> query, CompanyGetBinding binding)
    {
        string search = binding.Search?.Trim().ToLower();

        return query.WhereIf(!string.IsNullOrWhiteSpace(search),
            x => x.Name.ToLower().Contains(search) || x.ValueId.ToLower().Contains(search));
    }
}
