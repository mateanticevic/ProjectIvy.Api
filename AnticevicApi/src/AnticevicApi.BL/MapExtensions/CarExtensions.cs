using AnticevicApi.DL.DbContexts;
using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.Binding.Car;
using AnticevicApi.Model.Database.Main.Transport;
using System;

namespace AnticevicApi.BL.MapExtensions
{
    public static class CarExtensions
    {
        public static CarLog ToEntity(this CarLogBinding binding, MainContext db, CarLog entity = null)
        {
            if(entity == null)
            {
                entity = new CarLog();
            }

            entity.CarId = db.Cars.GetId(binding.CarValueId).Value;
            entity.Odometer = binding.Odometer;
            entity.Timestamp = DateTime.Now;

            return entity;
        }
    }
}
