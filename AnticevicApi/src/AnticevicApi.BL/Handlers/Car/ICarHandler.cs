using AnticevicApi.Model.Binding.Car;
using System.Collections.Generic;
using System;
using View = AnticevicApi.Model.View.Car;

namespace AnticevicApi.BL.Handlers.Car
{
    public interface ICarHandler : IHandler
    {
        void Create(string valueId, CarBinding car);

        DateTime CreateLog(CarLogBinding binding);

        IEnumerable<View.Car> Get();

        int GetLogCount(string carValueId);

        View.CarLog GetLatestLog(string carValueId);
    }
}
