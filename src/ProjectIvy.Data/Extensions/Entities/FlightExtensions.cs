using ProjectIvy.Model.Binding.Flight;
using ProjectIvy.Model.Database.Main.Transport;
using System.Linq;

namespace ProjectIvy.Data.Extensions.Entities
{
    public static class FlightExtensions
    {
        public static IQueryable<Flight> Where(this IQueryable<Flight> query, FlightGetBinding binding)
        {
            return query.WhereIf(binding.From.HasValue, x => x.DateOfDepartureLocal >= binding.From)
                        .WhereIf(binding.To.HasValue, x => x.DateOfDepartureLocal <= binding.To);
        }
    }
}
