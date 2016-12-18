using AnticevicApi.BL.MapExtensions;
using AnticevicApi.DL.DbContexts;
using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.Binding.Car;
using AnticevicApi.Model.View.Car;
using Microsoft.EntityFrameworkCore;
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
            using (var db = GetMainContext())
            {
                var entity = binding.ToEntity(db);

                db.CarLogs.Add(entity);
                db.SaveChanges();

                return entity.Timestamp;
            }
        }

        public int GetLogCount(string carValueId)
        {
            using (var db = GetMainContext())
            {
                return db.Cars.WhereUser(UserId)
                                    .Include(x => x.CarLogs)
                                    .SingleOrDefault(x => x.ValueId == carValueId)
                                    .CarLogs
                                    .Count;
            }
        }

        public CarLog GetLatestLog(string carValueId)
        {
            using (var db = GetMainContext())
            {
                var carLog = db.Cars.WhereUser(UserId)
                                    .Include(x => x.CarLogs)
                                    .SingleOrDefault(x => x.ValueId == carValueId)
                                    .CarLogs
                                    .OrderByDescending(x => x.Timestamp)
                                    .FirstOrDefault();

                return new CarLog(carLog);
            }
        }
    }
}
