using ProjectIvy.Model.Binding.Car;
using ProjectIvy.Model.Database.Main.Transport;
using System;
using System.Linq;

namespace ProjectIvy.Data.Extensions.Entities
{
    public static class CarLogExtensions
    {
        public static int? GetAproximateOdometer(this IQueryable<CarLog> logs, int carId, DateTime dateTime)
        {
            var lastLogBefore = logs.Where(x => x.CarId == carId && x.Timestamp < dateTime && x.Odometer.HasValue)
                                    .OrderByDescending(x => x.Timestamp)
                                    .FirstOrDefault();

            var firstLogAfter = logs.Where(x => x.CarId == carId && x.Timestamp > dateTime && x.Odometer.HasValue)
                                    .OrderBy(x => x.Timestamp)
                                    .FirstOrDefault();

            if (lastLogBefore == null)
                return null;

            if (firstLogAfter == null)
                return lastLogBefore.Odometer;

            return (int)(lastLogBefore.Odometer
                   + (firstLogAfter.Odometer - lastLogBefore.Odometer)
                   * (dateTime - lastLogBefore.Timestamp).TotalMilliseconds
                   / (firstLogAfter.Timestamp - lastLogBefore.Timestamp).TotalMilliseconds);
        }

        public static int? GetLastOdometer(this IQueryable<CarLog> logs, int carId)
            => logs.Where(x => x.Odometer.HasValue && x.CarId == carId)
                   .OrderByDescending(x => x.Odometer)
                   .FirstOrDefault()?
                   .Odometer;

        public static IQueryable<CarLog> Where(this IQueryable<CarLog> logs, CarLogGetBinding binding)
        {
            return logs.WhereTimestampInclusive(binding)
                       .WhereIf(binding.HasOdometer.HasValue, x => !(x.Odometer.HasValue ^ binding.HasOdometer.Value));
        }
    }
}
