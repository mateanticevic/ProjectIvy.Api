using System.Linq;
using ProjectIvy.Model.Binding.Stay;
using ProjectIvy.Model.Database.Main.Travel;

namespace ProjectIvy.Data.Extensions.Entities;

public static class StayExtensions
{
    public static IOrderedQueryable<Stay> OrderBy(this IQueryable<Stay> query, StayGetBinding b)
    {
        switch (b.OrderBy)
        {
            case StaySort.Date:
            default:
                return query.OrderBy(b.OrderAscending, x => x.Date);
        }
    }

    public static IQueryable<Stay> Where(this IQueryable<Stay> query, StayGetBinding b)
        => query.WhereIf(b.From.HasValue, x => x.Date >= b.From.Value)
                .WhereIf(b.To.HasValue, x => x.Date <= b.To.Value)
                .WhereIf(b.CityId, x => b.CityId.Contains(x.City.ValueId))
                .WhereIf(b.CountryId, x => b.CountryId.Contains(x.Country.ValueId));
}