using AnticevicApi.BL.MapExtensions;
using AnticevicApi.DL.DbContexts;
using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.Binding.Car;
using AnticevicApi.Model.View.Car;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace AnticevicApi.BL.Handlers
{
    public class CarHandler : Handler
    {
        public CarHandler(string connectionString, int userId) : base(connectionString, userId)
        {
        }

        public DateTime CreateLog(CarLogBinding binding)
        {
            using (var db = new MainContext(ConnectionString))
            {
                var entity = binding.ToEntity();

                db.CarLogs.Add(entity);
                db.SaveChanges();

                return entity.Timestamp;
            }
        }

        public int GetLogCount(string carValueId)
        {
            using (var db = new MainContext(ConnectionString))
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
            using (var db = new MainContext(ConnectionString))
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
