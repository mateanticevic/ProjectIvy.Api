using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geohash;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Geohash;

namespace ProjectIvy.Business.Handlers.Geohash
{
    public class GeohashHandler : Handler<GeohashHandler>, IGeohashHandler
    {
        public GeohashHandler(IHandlerContext<GeohashHandler> context) : base(context)
        {
        }

        public async Task<Model.View.Geohash.Geohash> GetGeohash(string geohashId)
        {
            using (var context = GetMainContext())
            {
                var query = context.Trackings.WhereUser(UserId)
                                                      .Where(x => x.Geohash.StartsWith(geohashId));

                var firstIn = (await query.OrderBy(x => x.Timestamp)
                                          .FirstOrDefaultAsync())?.Timestamp;

                if (firstIn is null)
                    return null;

                int totalCount = await query.CountAsync();

                var lastIn = totalCount == 1 ? null : (await query.OrderBy(x => x.Timestamp)
                      .OrderByDescending(x => x.Timestamp)
                      .FirstOrDefaultAsync())?.Timestamp;

                int dayCount = await query.GroupBy(x => x.Timestamp.Date)
                                           .CountAsync();

                return new Model.View.Geohash.Geohash()
                {
                    DayCount = dayCount,
                    FirstIn = firstIn.Value,
                    LastIn = lastIn.HasValue ? lastIn.Value : firstIn.Value,
                    TotalCount = totalCount
                };
            }
        }

        public async Task<IEnumerable<string>> GetGeohashes(GeohashGetBinding binding)
        {
            var geohasher = new Geohasher();

            using (var context = GetMainContext())
            {
                var neighbours = binding.Geohash is null ? null : geohasher.GetNeighbors(binding.Geohash);

                return await context.Trackings.WhereUser(UserId)
                                              .WhereTimestampInclusive(binding)
                                              .WhereIf(neighbours is not null, x => x.Geohash.StartsWith(binding.Geohash)
                                                     || x.Geohash.StartsWith(neighbours[Direction.North])
                                                     || x.Geohash.StartsWith(neighbours[Direction.South])
                                                     || x.Geohash.StartsWith(neighbours[Direction.East])
                                                     || x.Geohash.StartsWith(neighbours[Direction.West])
                                                     || x.Geohash.StartsWith(neighbours[Direction.NorthEast])
                                                     || x.Geohash.StartsWith(neighbours[Direction.NorthWest])
                                                     || x.Geohash.StartsWith(neighbours[Direction.SouthEast])
                                                     || x.Geohash.StartsWith(neighbours[Direction.SouthWest]))
                                              .Select(x => x.Geohash.Substring(0, binding.Precision))
                                              .Distinct()
                                              .ToListAsync();
            }
        }
    }
}
