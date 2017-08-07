using AnticevicApi.DL.DbContexts;
using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.Binding.Car;
using AnticevicApi.Model.Database.Main.Transport;
using System;

namespace AnticevicApi.BL.MapExtensions
{
    public static class CarExtensions
    {
        public static Car ToEntity(this CarBinding b, MainContext context, Car entity = null)
        {
            if (entity == null)
                entity = new Car();

            entity.ManufacturerId = context.Manufacturers.GetId(b.ManufacturerId).Value;
            entity.Model = b.Model;
            entity.ProductionYear = b.ProductionYear;

            return entity;
        }

        public static CarLog ToEntity(this CarLogBinding b, MainContext context, CarLog entity = null)
        {
            if (entity == null)
                entity = new CarLog();

            entity.CarId = context.Cars.GetId(b.CarValueId).Value;
            entity.Odometer = b.Odometer;
            entity.Timestamp = DateTime.Now;

            return entity;
        }
    }
}
