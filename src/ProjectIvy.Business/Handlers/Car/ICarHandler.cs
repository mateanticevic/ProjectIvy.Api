using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Car;
using ProjectIvy.Model.View.Car;
using View = ProjectIvy.Model.View.Car;

namespace ProjectIvy.Business.Handlers.Car;

public interface ICarHandler : IHandler
{
    Task Create(string valueId, CarBinding car);

    Task<DateTime> CreateLog(string carValueId, CarLogBinding binding);

    Task<string> CreateService(string carValueId, CarServiceBinding binding);

    Task CreateTorqueLog(string carValueId, CarLogTorqueBinding binding);

    Task<IEnumerable<View.Car>> Get();

    Task<View.Car> Get(string carId);

    Task<decimal> GetAverageConsumption(string carValueId);

    Task<IEnumerable<KeyValuePair<int, decimal>>> GetAverageConsumptionByYear(string carValueId);

    Task<IEnumerable<KeyValuePair<int, decimal>>> GetFuelByMonth(string carValueId);

    Task<IEnumerable<KeyValuePair<int, decimal>>> GetFuelByYear(string carValueId);

    Task<IEnumerable<CarFueling>> GetFuelings(string carValueId);

    Task<IEnumerable<KeyValuePair<int, int>>> GetKilometersByYear(string carValueId);

    Task<CarLog> GetLatestLog(CarLogGetBinding binding);

    Task<CarLog> GetLatestLog(string carValueId, CarLogGetBinding binding);

    Task<IEnumerable<View.CarLogBySession>> GetLogBySession(string carValueId, CarLogGetBinding binding);

    Task<int> GetLogCount(string carValueId);

    Task<IEnumerable<View.CarLog>> GetLogs(string carValueId, CarLogGetBinding binding);

    Task<IEnumerable<View.CarServiceInterval>> GetServiceIntervals(string carModelValueId);

    Task<IEnumerable<View.CarServiceType>> GetServiceTypes(string carModelValueId);

    Task NewFueling(string carValueId, CarFuelingBinding b);
}
