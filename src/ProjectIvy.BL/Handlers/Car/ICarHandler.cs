using ProjectIvy.Model.Binding.Car;
using System.Collections.Generic;
using System;
using View = ProjectIvy.Model.View.Car;

namespace ProjectIvy.BL.Handlers.Car
{
    public interface ICarHandler : IHandler
    {
        void Create(string valueId, CarBinding car);

        DateTime CreateLog(CarLogBinding binding);

        void CreateTorqueLog(string carValueId, CarLogTorqueBinding binding);

        IEnumerable<View.Car> Get();

        int GetLogCount(string carValueId);

        View.CarLog GetLatestLog(string carValueId);
    }
}
