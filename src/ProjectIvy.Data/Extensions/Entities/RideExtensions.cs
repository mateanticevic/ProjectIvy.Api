using ProjectIvy.Model.Binding.Ride;
using ProjectIvy.Model.Database.Main.Transport;
using System.Linq;

namespace ProjectIvy.Data.Extensions.Entities;

public static class RideExtensions
{
    public static IQueryable<Ride> Where(this IQueryable<Ride> query, RideGetBinding binding)
    {
        return query.WhereIf(binding.From.HasValue, x => x.DateOfDeparture >= binding.From)
                    .WhereIf(binding.To.HasValue, x => x.DateOfDeparture <= binding.To);
    }
}
