using ProjectIvy.Model.Binding.Car;
using ProjectIvy.Model.Database.Main.Transport;
using System.Linq;

namespace ProjectIvy.Data.Extensions.Entities
{
    public static class CarLogExtensions
    {
        public static IQueryable<CarLog> Where(this IQueryable<CarLog> logs, CarLogGetBinding binding)
        {
            return logs.WhereTimestampInclusive(binding)
                       .WhereIf(binding.HasOdometer.HasValue, x => !(x.Odometer.HasValue ^ binding.HasOdometer.Value));
        }
    }
}
