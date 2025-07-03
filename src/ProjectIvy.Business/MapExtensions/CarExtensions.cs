﻿using ProjectIvy.Common.Helpers;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Car;
using ProjectIvy.Model.Database.Main.Transport;
using System;

namespace ProjectIvy.Business.MapExtensions;

public static class CarExtensions
{
    public static Car ToEntity(this CarBinding b, MainContext context, Car entity = null)
    {
        entity = entity ?? new Car();

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
            ConsumptionPer100Km = b.Kff1207,
            CoolantTemperature = (short?)b.K5,
            EngineRpm = (short?)b.Kc,
            ExhaustGasTemperature1 = (short?)b.K78,
            FuelRailPressure = (int?)b.K23,
            FuelUsed = b.Kff1271,
            IntakeAirTemperature = (short?)b.Kf,
            IntakeManifoldPressure = (short?)b.Kb,
            MassAirFlowRate = b.K10,
            Session = b.Session,
            SpeedKmh = (short?)b.Kd,
            Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(b.Time).UtcDateTime,
            TransmissionTemperature1 = (short?)b.Kfe1805,
            TripDistance = b.Kff1204.HasValue ? (int)(b.Kff1204 * 1000) : (int?)null
        };
    }

    public static CarService ToEntity(this CarServiceBinding b, MainContext context, CarService entity = null)
    {
        if (entity == null)
            entity = new CarService();

        entity.CarServiceTypeId = context.CarServiceTypes.GetId(b.TypeId).Value;
        entity.Date = b.Date;
        entity.Description = b.Description;
        entity.ValueId = IdHelper.Generate();

        return entity;
    }
}
