using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Airline;
using ProjectIvy.Model.View.Airline;

namespace ProjectIvy.Business.Handlers.Airport;

public class AirlineHandler : Handler<AirlineHandler>, IAirlineHandler
{
    public AirlineHandler(IHandlerContext<AirlineHandler> context) : base(context)
    {
    }

    public async Task<IEnumerable<Airline>> Get(AirlineGetBinding binding)
    {
        using var context = GetMainContext();

        return await context.Airlines.WhereIf(binding.Search, x => x.Name.ToLower().Contains(binding.Search.ToLower()))
                                     .Select(x => new Airline(x))
                                     .ToListAsync();
    }
}
