using ProjectIvy.BL.Exceptions;
using ProjectIvy.BL.MapExtensions;
using ProjectIvy.DL.Extensions;
using ProjectIvy.Model.Binding.Car;
using View = ProjectIvy.Model.View.Car;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ProjectIvy.BL.Handlers.Car
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
                var lastEntry = GetLatestLog(binding.CarValueId);

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

        public View.CarLog GetLatestLog(string carValueId)
        {
            using (var db = GetMainContext())
            {
                var carLog = db.Cars.WhereUser(User.Id)
                                    .Include(x => x.CarLogs)
                                    .SingleOrDefault(x => x.ValueId == carValueId)
                                    .CarLogs
                                    .OrderByDescending(x => x.Timestamp)
                                    .FirstOrDefault();

                return new View.CarLog(carLog);
            }
        }
    }
}
