using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Airline;
using ProjectIvy.Model.View.Airline;

namespace ProjectIvy.Business.Handlers.Airport;

public interface IAirlineHandler
{
    Task<IEnumerable<Airline>> Get(AirlineGetBinding binding);
}
