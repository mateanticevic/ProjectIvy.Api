using AnticevicApi.BL.Exceptions;
using AnticevicApi.BL.MapExtensions;
using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.Binding.Car;
using View = AnticevicApi.Model.View.Car;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;

namespace AnticevicApi.BL.Handlers.Car
{
    public class CarHandler : Handler<CarHandler>, ICarHandler
    {
        public CarHandler(IHandlerContext<CarHandler> context) : base(context)
        {
        }

        public DateTime CreateLog(CarLogBinding binding)
        {
            using (var context = GetMainContext())
            {
                var lastEntry = GetLatestLog(binding.CarValueId);

                if(binding.Odometer < lastEntry.Odometer)
                {
                    throw new InvalidRequestException($"Odometer must be {lastEntry.Odometer}km or higher.");
                }

                var entity = binding.ToEntity(context);

                context.CarLogs.Add(entity);
                context.SaveChanges();

                return entity.Timestamp;
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
