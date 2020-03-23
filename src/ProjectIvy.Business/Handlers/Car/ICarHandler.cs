using ProjectIvy.Model.Binding.Car;
using System;
using System.Collections.Generic;
using View = ProjectIvy.Model.View.Car;

namespace ProjectIvy.Business.Handlers.Car
{
    public interface ICarHandler : IHandler
    {
        void Create(string valueId, CarBinding car);

        DateTime CreateLog(CarLogBinding binding);

        void CreateTorqueLog(string carValueId, CarLogTorqueBinding binding);

        IEnumerable<View.Car> Get();

        View.Car Get(string carId);

        IEnumerable<View.CarLogBySession> GetLogBySession(string carValueId, CarLogGetBinding binding);

        int GetLogCount(string carValueId);

        View.CarLog GetLatestLog(string carValueId, CarLogGetBinding binding);
    }
}
