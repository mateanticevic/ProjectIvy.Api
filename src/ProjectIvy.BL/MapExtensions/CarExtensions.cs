using ProjectIvy.DL.DbContexts;
using ProjectIvy.DL.Extensions;
using ProjectIvy.Model.Binding.Car;
using ProjectIvy.Model.Database.Main.Transport;
using System;

namespace ProjectIvy.BL.MapExtensions
{
    public static class CarExtensions
    {
        public static Car ToEntity(this CarBinding b, MainContext context, Car entity = null)
        {
            entity = entity ?? new Car();

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

        public static CarLog ToEntity(this CarLogTorqueBinding b)
        {
            return new CarLog()
            {
                AccelerationAxisX = b.Kff1220,
                AccelerationAxisY = b.Kff1221,
                AccelerationAxisZ = b.Kff1222,
                AccelerationTotal = b.Kff1223,
                AmbientAirTemperature = (short?)b.K46,
                BarometricPressure = (short?)b.K33,
                CoolantTemperature = (short?)b.K5,
                EngineRpm = (short?)b.Kc,
                IntakeAirTemperature = (short?)b.Kf,
                IntakeManifoldPressure = (short?)b.Kb,
                Session = b.Session,
                SpeedKmh = (short?)b.Kd,
                Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(b.Time).UtcDateTime,
                TripDistance = b.Kff1204.HasValue ? (int)(b.Kff1204 * 1000) : (int?)null
            };
        }
    }
}
