using AnticevicApi.DL.Helpers;
using AnticevicApi.Model.Binding.Car;
using AnticevicApi.Model.Database.Main.Transport;
using System;

namespace AnticevicApi.BL.MapExtensions
{
    public static class CarExtensions
    {
        public static CarLog ToEntity(this CarLogBinding binding, CarLog entity = null)
        {
            if(entity == null)
            {
                entity = new CarLog();
            }

            entity.CarId = CarHelper.GetId(binding.CarValueId);
            entity.Odometer = binding.Odometer;
            entity.Timestamp = DateTime.Now;

            return entity;
        }
    }
}
