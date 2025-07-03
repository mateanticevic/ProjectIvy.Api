using System.Linq;
using ProjectIvy.Model.Binding.Beer;
using ProjectIvy.Model.Database.Main.Beer;

namespace ProjectIvy.Data.Extensions.Entities;

public static class BeerExtensions
{
    public static IOrderedQueryable<Beer> OrderBy(this IQueryable<Beer> query, BeerGetBinding binding)
    {
        switch (binding.OrderBy)
        {
            case BeerSort.Abv:
                return query.OrderBy(binding.OrderAscending, x => x.Abv);
            default:
                return query.OrderBy(binding.OrderAscending, x => x.Name);
        }
    }
}
