using ProjectIvy.Model.Binding.Car;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.Car;

namespace ProjectIvy.Business.Handlers.Car
{
    public interface ICarHandler : IHandler
    {
        void Create(string valueId, CarBinding car);

        DateTime CreateLog(CarLogBinding binding);

        Task<string> CreateService(string carValueId, CarServiceBinding binding);

        void CreateTorqueLog(string carValueId, CarLogTorqueBinding binding);

        IEnumerable<View.Car> Get();

        View.Car Get(string carId);

        IEnumerable<View.CarLogBySession> GetLogBySession(string carValueId, CarLogGetBinding binding);

        int GetLogCount(string carValueId);

        Task<IEnumerable<View.CarLog>> GetLogs(string carValueId, CarLogGetBinding binding);

        View.CarLog GetLatestLog(CarLogGetBinding binding);

        View.CarLog GetLatestLog(string carValueId, CarLogGetBinding binding);

        Task<IEnumerable<View.CarServiceInterval>> GetServiceIntervals(string carModelValueId);

        Task<IEnumerable<View.CarServiceType>> GetServiceTypes(string carModelValueId);
    }
}
