using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.Exceptions;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Model.Binding.Car;
using System;
using System.Collections.Generic;
using System.Linq;
using View = ProjectIvy.Model.View.Car;

namespace ProjectIvy.Business.Handlers.Car
{
    public class CarHandler : Handler<CarHandler>, ICarHandler
    {
        public CarHandler(IHandlerContext<CarHandler> context) : base(context)
        {
        }

        public void Create(string valueId, CarBinding car)
        {
            using (var context = GetMainContext())
            {
                var entity = car.ToEntity(context);
                entity.ValueId = valueId;
                entity.UserId = User.Id;

                context.Cars.Add(entity);
                context.SaveChanges();
            }
        }

        public DateTime CreateLog(CarLogBinding binding)
        {
            using (var context = GetMainContext())
            {
                var lastEntry = GetLatestLog(binding.CarValueId, new CarLogGetBinding() { HasOdometer = true });

                if (binding.Odometer < lastEntry.Odometer)
                {
                    throw new InvalidRequestException($"Odometer must be {lastEntry.Odometer}km or higher.");
                }

                var entity = binding.ToEntity(context);

                context.CarLogs.Add(entity);
                context.SaveChanges();

                return entity.Timestamp;
            }
        }

        public void CreateTorqueLog(string carValueId, CarLogTorqueBinding binding)
        {
            using (var context = GetMainContext())
            {
                int? carId = context.Cars.GetId(carValueId);

                var entity = binding.ToEntity();
                entity.CarId = carId.Value;
                context.CarLogs.Add(entity);
                context.SaveChanges();
            }
        }

        public IEnumerable<View.Car> Get()
        {
            using (var context = GetMainContext())
            {
                return context.Cars.WhereUser(User)
                                   .ToList()
                                   .Select(x => new View.Car(x))
                                   .ToList();
            }
        }

        public IEnumerable<View.CarLogBySession> GetLogBySession(string carValueId, CarLogGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Cars.WhereUser(User)
                                   .Include(x => x.CarLogs)
                                   .SingleOrDefault(x => x.ValueId == carValueId)
                                   .CarLogs
                                   .AsQueryable()
                                   .Where(binding)
                                   .Where(x => !string.IsNullOrEmpty(x.Session))
                                   .GroupBy(x => x.Session)
                                   .Select(x => new View.CarLogBySession()
                                   {
                                       Count = x.Count(),
                                       Distance = x.Max(y => y.TripDistance),
                                       End = x.Max(y => y.Timestamp),
                                       FuelUsed = x.Max(y => y.FuelUsed),
                                       MaxEngineRpm = x.Max(y => y.EngineRpm),
                                       MaxSpeed = x.Max(y => y.SpeedKmh),
                                       Session = x.Key,
                                       Start = x.Min(y => y.Timestamp)
                                   })
                                   .OrderByDescending(x => x.End);
            }

        }

        public int GetLogCount(string carValueId)
        {
            using (var db = GetMainContext())
            {
                return db.Cars.WhereUser(User.Id)
                                    .Include(x => x.CarLogs)
                                    .SingleOrDefault(x => x.ValueId == carValueId)
                                    .CarLogs
                                    .Count;
            }
        }

        public View.CarLog GetLatestLog(string carValueId, CarLogGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                var carLog = db.Cars.WhereUser(User.Id)
                                    .Include(x => x.CarLogs)
                                    .SingleOrDefault(x => x.ValueId == carValueId)
                                    .CarLogs
                                    .WhereIf(binding.HasOdometer.HasValue, x => x.Odometer.HasValue == binding.HasOdometer.Value)
                                    .OrderByDescending(x => x.Timestamp)
                                    .FirstOrDefault();

                return new View.CarLog(carLog);
            }
        }
    }
}
