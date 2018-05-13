using ProjectIvy.Model.Binding.Flight;
using ProjectIvy.Model.Database.Main.Transport;
using System.Linq;

namespace ProjectIvy.DL.Extensions.Entities
{
    public static class FlightExtensions
    {
        public static IQueryable<Flight> Where(this IQueryable<Flight> query, FlightGetBinding binding)
        {
            return query.WhereIf(binding.From.HasValue, x => x.DateOfDeparture.Date >= binding.From)
                        .WhereIf(binding.To.HasValue, x => x.DateOfDeparture.Date <= binding.To);
        }
    }
}
