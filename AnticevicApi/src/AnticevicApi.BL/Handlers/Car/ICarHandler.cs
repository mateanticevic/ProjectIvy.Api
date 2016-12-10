using AnticevicApi.Model.Binding.Car;
using AnticevicApi.Model.View.Car;
using System;

namespace AnticevicApi.BL.Handlers.Car
{
    public interface ICarHandler : IHandler
    {
        DateTime CreateLog(CarLogBinding binding);

        int GetLogCount(string carValueId);

        CarLog GetLatestLog(string carValueId);
    }
}
