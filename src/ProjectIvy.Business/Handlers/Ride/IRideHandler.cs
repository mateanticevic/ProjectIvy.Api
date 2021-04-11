using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Ride;

namespace ProjectIvy.Business.Handlers.Ride
{
    public interface IRideHandler
    {
        Task Create(RideBinding binding);

        Task<IEnumerable<Model.View.Ride.Ride>> GetRides();
    }
}