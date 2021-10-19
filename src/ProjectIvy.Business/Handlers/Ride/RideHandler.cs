using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Ride;
using ProjectIvy.Business.MapExtensions;
using View = ProjectIvy.Model.View;
using ProjectIvy.Data.Extensions.Entities;

namespace ProjectIvy.Business.Handlers.Ride
{
    public class RideHandler : Handler<RideHandler>, IRideHandler
    {
        public RideHandler(IHandlerContext<RideHandler> context) : base(context)
        {
        }

        public async Task Create(RideBinding binding)
        {
            using (var context = GetMainContext())
            {
                var entity = binding.ToEntity(context);
                entity.UserId = UserId.Value;

                await context.Rides.AddAsync(entity);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<View.Ride.Ride>> GetRides(RideGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return await context.Rides
                                    .WhereUser(UserId.Value)
                                    .Where(binding)
                                    .Include(x => x.DestinationCity)
                                    .Include(x => x.OriginCity)
                                    .OrderBy(x => x.DateOfDeparture)
                                    .Select(x => new View.Ride.Ride(x))
                                    .ToListAsync();
            }
        }
    }
}
