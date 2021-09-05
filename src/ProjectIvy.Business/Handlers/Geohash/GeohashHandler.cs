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

        public async Task<IEnumerable<string>> GetGeohashes(GeohashGetBinding binding)
        {
            var geohasher = new Geohasher();

            using (var context = GetMainContext())
            {
                if (binding.Geohash is null)
                {
                    return await context.UserGeohashes.WhereUser(User)
                                                      .Select(x => x.Hash.Substring(0, binding.Precision))
                                                      .Distinct()
                                                      .ToListAsync();
                }

                var neighbours = geohasher.GetNeighbors(binding.Geohash);

                return await context.UserGeohashes.WhereUser(User)
                                                  .Where(x => x.Hash.StartsWith(binding.Geohash)
                                                         || x.Hash.StartsWith(neighbours[Direction.North])
                                                         || x.Hash.StartsWith(neighbours[Direction.South])
                                                         || x.Hash.StartsWith(neighbours[Direction.East])
                                                         || x.Hash.StartsWith(neighbours[Direction.West])
                                                         || x.Hash.StartsWith(neighbours[Direction.NorthEast])
                                                         || x.Hash.StartsWith(neighbours[Direction.NorthWest])
                                                         || x.Hash.StartsWith(neighbours[Direction.SouthEast])
                                                         || x.Hash.StartsWith(neighbours[Direction.SouthWest]))
                                                  .Select(x => x.Hash.Substring(0, binding.Precision))
                                                  .Distinct()
                                                  .ToListAsync();
            }
        }
    }
}
