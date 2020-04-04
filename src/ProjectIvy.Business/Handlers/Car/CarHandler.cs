using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.Exceptions;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Model.Binding.Car;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                if (string.IsNullOrWhiteSpace(binding.CarValueId))
                    binding.CarValueId = context.Users.Include(x => x.DefaultCar).SingleOrDefault(x => x.Id == User.Id).DefaultCar.ValueId;

                var lastEntry = GetLatestLog(binding.CarValueId, new CarLogGetBinding() { HasOdometer = true });

                if (lastEntry != null && binding.Odometer < lastEntry.Odometer)
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
                                   .Include(x => x.CarModel)
                                   .ThenInclude(x => x.Manufacturer)
                                   .ToList()
                                   .Select(x => new View.Car(x))
                                   .ToList();
            }
        }

        public View.Car Get(string carId)
        {
            using (var context = GetMainContext())
            {
                var car = context.Cars.WhereUser(User)
                                      .Include(x => x.CarServices)
                                      .Include($"{nameof(Model.Database.Main.Transport.Car.CarServices)}.{nameof(Model.Database.Main.Transport.CarServiceType)}")
                                      .Include(x => x.CarModel)
                                      .ThenInclude(x => x.Manufacturer)
                                      .SingleOrDefault(x => x.ValueId == carId);

                return new View.Car(car);
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

                return carLog == null ? null : new View.CarLog(carLog);
            }
        }

        public async Task<IEnumerable<View.CarServiceInterval>> GetServiceIntervals(string carModelValueId)
        {
            using (var context = GetMainContext())
            {
                int? carModelId = context.CarModels.GetId(carModelValueId);

                return await context.CarServiceIntervals.Where(x => x.CarModelId == carModelId)
                                                        .Include(x => x.CarServiceType)
                                                        .Select(x => new View.CarServiceInterval(x))
                                                        .ToListAsync();
            }
        }
    }
}
