using ProjectIvy.Model.Binding.Trip;
using ProjectIvy.Model.Database.Main.Travel;
using System.Linq;

namespace ProjectIvy.Data.Extensions.Entities
{
    public static class TripExtensions
    {
        public static IOrderedQueryable<Trip> OrderBy(this IQueryable<Trip> query, TripGetBinding b)
        {
            switch (b.OrderBy)
            {
                case TripSort.Date:
                    return query.OrderBy(b.OrderAscending, x => x.TimestampStart);
                case TripSort.Duration:
                    return query.OrderBy(b.OrderAscending, x => x.TimestampEnd.Subtract(x.TimestampStart).TotalMinutes);
                default:
                    return query.OrderBy(b.OrderAscending, x => x.TimestampStart);
            }
        }

        public static IQueryable<Trip> Where(this IQueryable<Trip> query, TripGetBinding b)
            => query.WhereIf(b.Search, x => x.Name.ToLower().Contains(b.Search.ToLower()))
                    .WhereIf(b.From.HasValue, x => x.TimestampEnd > b.From.Value)
                    .WhereIf(b.To.HasValue, x => x.TimestampStart < b.To.Value)
                    .WhereIf(b.CityId, x => x.Cities.Select(y => y.ValueId).Any(y => b.CityId.Contains(y)))
                    .WhereIf(b.IsDomestic.HasValue, x => x.IsDomestic == b.IsDomestic)
                    .WhereIf(b.CountryId, x => x.Cities.Select(y => y.Country.ValueId).Any(y => b.CountryId.Contains(y)));
    }
}
