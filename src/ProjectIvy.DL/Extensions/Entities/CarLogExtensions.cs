using ProjectIvy.Model.Binding.Car;
using ProjectIvy.Model.Database.Main.Transport;
using System.Linq;

namespace ProjectIvy.DL.Extensions.Entities
{
    public static class CarLogExtensions
    {
        public static IQueryable<CarLog> Where(this IQueryable<CarLog> logs, CarLogGetBinding binding)
        {
            return logs.WhereTimestampInclusive(binding);
        }
    }
}
