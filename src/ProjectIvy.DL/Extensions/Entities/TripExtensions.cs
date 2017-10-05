using ProjectIvy.Model.Binding.Trip;
using ProjectIvy.Model.Database.Main.Travel;
using System.Linq;

namespace ProjectIvy.DL.Extensions.Entities
{
    public static class TripExtensions
    {
        public static IOrderedQueryable<Trip> OrderBy(this IQueryable<Trip> query, TripGetBinding binding)
        {
            switch (binding.OrderBy)
            {
                case TripSort.Date:
                    return query.OrderBy(binding.OrderAscending, x => x.TimestampStart);
                case TripSort.Duration:
                    return query.OrderBy(binding.OrderAscending, x => x.TimestampEnd.Subtract(x.TimestampStart).TotalMinutes);
                default:
                    return query.OrderBy(binding.OrderAscending, x => x.TimestampStart);
            }
        }
    }
}
