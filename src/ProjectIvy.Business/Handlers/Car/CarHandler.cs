using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.Exceptions;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Model.Binding.Car;
using ProjectIvy.Model.View.Car;
using View = ProjectIvy.Model.View.Car;

namespace ProjectIvy.Business.Handlers.Car;

public class CarHandler : Handler<CarHandler>, ICarHandler
{
    public CarHandler(IHandlerContext<CarHandler> context) : base(context) { }

    public async Task Create(string valueId, CarBinding car)
    {
        using var context = GetMainContext();
        var entity = car.ToEntity(context);
        entity.ValueId = valueId;
        entity.UserId = UserId;

        await context.Cars.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task<DateTime> CreateLog(string carValueId, CarLogBinding binding)
    {
        using var context = GetMainContext();
        if (string.IsNullOrWhiteSpace(carValueId))
            carValueId = (await context.Users.Include(x => x.DefaultCar).SingleOrDefaultAsync(x => x.Id == UserId)).DefaultCar.ValueId;

        var lastEntry = await GetLatestLog(carValueId, new CarLogGetBinding() { HasOdometer = true });

        if (lastEntry != null && binding.Odometer < lastEntry.Odometer)
            throw new InvalidRequestException($"Odometer must be {lastEntry.Odometer}km or higher.");

        var entity = await binding.ToEntity(carValueId, context);

        await context.CarLogs.AddAsync(entity);
        await context.SaveChangesAsync();

        return entity.Timestamp;
    }

    public async Task<string> CreateService(string carValueId, CarServiceBinding binding)
    {
        using var context = GetMainContext();
        var entity = await binding.ToEntity(context);
        entity.CarId = (await context.Cars.GetIdAsync(carValueId)).Value;

        await context.CarServices.AddAsync(entity);
        await context.SaveChangesAsync();

        return entity.ValueId;
    }

    public async Task CreateTorqueLog(string carValueId, CarLogTorqueBinding binding)
    {
        using var context = GetMainContext();
        int? carId = await context.Cars.GetIdAsync(carValueId);

        var entity = binding.ToEntity();
        entity.CarId = carId.Value;
        await context.CarLogs.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<View.Car>> Get()
    {
        using var context = GetMainContext();
        return (await context.Cars.WhereUser(UserId)
                           .Include(x => x.CarModel)
                           .ThenInclude(x => x.Manufacturer)
                           .ToListAsync())
                           .Select(x => new View.Car(x))
                           .ToList();
    }

    public async Task<View.Car> Get(string carId)
    {
        using var context = GetMainContext();
        var car = await context.Cars.WhereUser(UserId)
                              .Include(x => x.CarServices)
                              .Include($"{nameof(Model.Database.Main.Transport.Car.CarServices)}.{nameof(Model.Database.Main.Transport.CarServiceType)}")
                              .Include(x => x.CarModel)
                              .ThenInclude(x => x.Manufacturer)
                              .SingleOrDefaultAsync(x => x.ValueId == carId);

        var lastLog = await context.CarLogs.Where(x => x.Odometer.HasValue && x.CarId == car.Id)
                                     .OrderByDescending(x => x.Odometer)
                                     .FirstOrDefaultAsync();

        int averageKmPerDay = lastLog.Odometer.Value / lastLog.Timestamp.Subtract(car.FirstRegistered.Value).Days;

        var serviceDue = new List<CarServiceDue>();
        foreach (var serviceInterval in await context.CarServiceIntervals
                                               .Include(x => x.CarServiceType)
                                               .Where(x => x.CarModelId == car.CarModelId && (x.Days.HasValue || x.Range.HasValue))
                                               .ToListAsync())
        {
            var lastService = car.CarServices.Where(x => x.CarServiceTypeId == serviceInterval.CarServiceTypeId)
                                             .OrderByDescending(x => x.Date)
                                             .FirstOrDefault();

            if (lastService == null && car.FirstRegistered.HasValue)
            {
                int? dueIn = serviceInterval.Range.HasValue ? serviceInterval.Range - lastLog.Odometer : null;

                serviceDue.Add(new CarServiceDue
                {
                    DueAt = serviceInterval.Range.HasValue ? serviceInterval.Range : null,
                    DueIn = dueIn,
                    DueBefore = serviceInterval.Days.HasValue ? car.FirstRegistered.Value.AddDays(serviceInterval.Days.Value) : null,
                    DueBeforeApprox = dueIn.HasValue ? DateTime.Now.AddDays(dueIn.Value / averageKmPerDay) : null,
                    ServiceType = new CarServiceType(serviceInterval.CarServiceType)
                });
            }
            else
            {
                int? aproximateOdometer = await context.CarLogs.GetAproximateOdometerAsync(car.Id, lastService.Date);
                int? dueIn = serviceInterval.Range.HasValue ? aproximateOdometer + serviceInterval.Range - lastLog.Odometer : null;

                serviceDue.Add(new CarServiceDue()
                {
                    DueAt = serviceInterval.Range.HasValue ? aproximateOdometer + serviceInterval.Range.Value : null,
                    DueIn = dueIn,
                    DueBefore = serviceInterval.Days.HasValue ? lastService.Date.AddDays(serviceInterval.Days.Value) : null,
                    DueBeforeApprox = dueIn.HasValue ? DateTime.Now.AddDays(dueIn.Value / averageKmPerDay) : null,
                    ServiceType = new CarServiceType(lastService.CarServiceType)
                });
            }
        }

        var services = new List<CarService>();
        foreach (var service in car.CarServices.OrderByDescending(x => x.Date))
        {
            services.Add(new CarService(service)
            {
                Odometer = await context.CarLogs.GetAproximateOdometerAsync(car.Id, service.Date) ?? 0
            });
        }

        return new(car)
        {
            Services = services,
            ServiceDue = serviceDue
        };
    }

    public async Task<decimal> GetAverageConsumption(string carValueId)
    {
        using var context = GetMainContext();

        var car = await context.Cars.WhereUser(UserId).SingleOrDefaultAsync(x => x.ValueId == carValueId);
        decimal totalFuel = await context.CarFuelings.Where(x => x.CarId == car.Id).SumAsync(x => x.AmountInLiters);

        var odometerQuery = context.CarLogs.Where(x => x.CarId == car.Id && x.Odometer.HasValue);
        int firstOdometerValue = (await odometerQuery.Where(x => x.Timestamp >= car.OwnerSince.Value).OrderBy(x => x.Timestamp).FirstOrDefaultAsync()).Odometer.Value;
        int lastOdometerValue = (await odometerQuery.OrderByDescending(x => x.Timestamp).FirstOrDefaultAsync()).Odometer.Value;

        return Math.Round((totalFuel * 100) / (lastOdometerValue - firstOdometerValue), 2);
    }

    public async Task<IEnumerable<KeyValuePair<int, decimal>>> GetAverageConsumptionByYear(string carValueId)
    {
        var kilometersByYear = await GetKilometersByYear(carValueId);
        var fuelByYear = await GetFuelByYear(carValueId);

        return fuelByYear.Join(kilometersByYear, x => x.Key, x => x.Key, (x, y) => new KeyValuePair<int, decimal>(x.Key, Math.Round(x.Value / (y.Value / 100), 2)));
    }

    public async Task<IEnumerable<KeyValuePair<int, decimal>>> GetFuelByMonth(string carValueId)
    {
        using var context = GetMainContext();
        return (await context.Cars.WhereUser(UserId)
                           .Include(x => x.CarFuelings)
                           .SingleAsync(x => x.ValueId == carValueId))
                           .CarFuelings
                           .GroupBy(x => x.Timestamp.Month)
                           .Select(x => new KeyValuePair<int, decimal>(x.Key, x.Sum(y => y.AmountInLiters)))
                           .OrderByDescending(x => x.Key)
                           .ToList();
    }

    public async Task<IEnumerable<KeyValuePair<int, decimal>>> GetFuelByYear(string carValueId)
    {
        using var context = GetMainContext();
        return (await context.Cars.WhereUser(UserId)
                           .Include(x => x.CarFuelings)
                           .SingleAsync(x => x.ValueId == carValueId))
                           .CarFuelings
                           .GroupBy(x => x.Timestamp.Year)
                           .Select(x => new KeyValuePair<int, decimal>(x.Key, x.Sum(y => y.AmountInLiters)))
                           .OrderBy(x => x.Key)
                           .ToList();
    }

    public async Task<IEnumerable<CarFueling>> GetFuelings(string carValueId)
    {
        using var context = GetMainContext();

        return (await context.Cars.WhereUser(UserId)
                           .Include(x => x.CarFuelings)
                           .SingleAsync(x => x.ValueId == carValueId))
                           .CarFuelings
                           .OrderByDescending(x => x.Timestamp)
                           .Select(x => new CarFueling(x))
                           .ToList();
    }

    public async Task<IEnumerable<KeyValuePair<int, int>>> GetKilometersByYear(string carValueId)
    {
        using var context = GetMainContext();
        var car = await context.Cars.WhereUser(UserId).SingleAsync(x => x.ValueId == carValueId);

        var years = Enumerable.Range(car.FirstRegistered.Value.Year, DateTime.Now.Year - car.FirstRegistered.Value.Year + 1);

        var kilometersByYear = new List<KeyValuePair<int, int>>();

        foreach (int year in years)
        {
            int fromOdometer = await context.CarLogs.GetAproximateOdometerAsync(car.Id, new DateTime(year, 1, 1)) ?? 0;
            int toOdometer = await context.CarLogs.GetAproximateOdometerAsync(car.Id, new DateTime(year + 1, 1, 1)) ?? 0;

            kilometersByYear.Add(new KeyValuePair<int, int>(year, toOdometer - fromOdometer));
        }

        return kilometersByYear.OrderBy(x => x.Key);
    }

    public async Task<View.CarLog> GetLatestLog(CarLogGetBinding binding)
    {
        using var context = GetMainContext();
        string carValueId = (await context.Users.Include(x => x.DefaultCar).SingleOrDefaultAsync(x => x.Id == UserId)).DefaultCar.ValueId;
        return await GetLatestLog(carValueId, binding);
    }

    public async Task<View.CarLog> GetLatestLog(string carValueId, CarLogGetBinding binding)
    {
        using var db = GetMainContext();
        int? carId = await db.Cars.WhereUser(UserId).GetIdAsync(carValueId);

        var carLog = await db.CarLogs
                       .Where(x => x.CarId == carId)
                       .WhereIf(binding.HasOdometer.HasValue, x => x.Odometer.HasValue == binding.HasOdometer.Value)
                       .OrderByDescending(x => x.Timestamp)
                       .FirstOrDefaultAsync();

        return carLog == null ? null : new View.CarLog(carLog);
    }

    public async Task<IEnumerable<View.CarLogBySession>> GetLogBySession(string carValueId, CarLogGetBinding binding)
    {
        using var context = GetMainContext();
        return (await context.Cars.WhereUser(UserId)
                           .Include(x => x.CarLogs)
                           .SingleOrDefaultAsync(x => x.ValueId == carValueId))
                           .CarLogs
                           .AsQueryable()
                           .Where(binding)
                           .Where(x => !string.IsNullOrEmpty(x.Session))
                           .GroupBy(x => x.Session)
                           .Select(x => new View.CarLogBySession()
                           {
                               Count = x.Count(),
                               Distance = x.Max(y => y.TripDistance),
                               End = x.Max(y => y.Timestamp),
                               FuelUsed = x.Max(y => y.FuelUsed),
                               MaxEngineRpm = x.Max(y => y.EngineRpm),
                               MaxSpeed = x.Max(y => y.SpeedKmh),
                               Session = x.Key,
                               Start = x.Min(y => y.Timestamp)
                           })
                           .OrderByDescending(x => x.End);

    }

    public async Task<int> GetLogCount(string carValueId)
    {
        using var db = GetMainContext();
        return (await db.Cars.WhereUser(UserId)
                            .Include(x => x.CarLogs)
                            .SingleOrDefaultAsync(x => x.ValueId == carValueId))
                            .CarLogs
                            .Count;
    }

    public async Task<IEnumerable<View.CarLog>> GetLogs(string carValueId, CarLogGetBinding binding)
    {
        using var context = GetMainContext();
        int? carId = await context.Cars.GetIdAsync(carValueId);

        return await context.CarLogs
                            .Where(x => x.CarId == carId.Value)
                            .Where(binding)
                            .OrderBy(x => x.Timestamp)
                            .Select(x => new View.CarLog(x))
                            .ToListAsync();
    }

    public async Task<IEnumerable<View.CarServiceInterval>> GetServiceIntervals(string carModelValueId)
    {
        using var context = GetMainContext();
        int? carModelId = await context.CarModels.GetIdAsync(carModelValueId);

        return await context.CarServiceIntervals.Where(x => x.CarModelId == carModelId)
                                                .Include(x => x.CarServiceType)
                                                .Select(x => new View.CarServiceInterval(x))
                                                .ToListAsync();
    }

    public async Task<IEnumerable<View.CarServiceType>> GetServiceTypes(string carModelValueId)
    {
        using var context = GetMainContext();
        int? carModelId = await context.CarModels.GetIdAsync(carModelValueId);

        return await context.CarServiceIntervals.Where(x => x.CarModelId == carModelId)
                                                .Include(x => x.CarServiceType)
                                                .OrderBy(x => x.CarServiceType.Name)
                                                .Select(x => new View.CarServiceType(x.CarServiceType))
                                                .ToListAsync();
    }

    public async Task NewFueling(string carValueId, CarFuelingBinding b)
    {
        using var context = GetMainContext();

        int carId = (await context.Cars.WhereUser(UserId).GetIdAsync(carValueId)).Value;
        var expense = await context.Expenses.WhereUser(UserId)
                                            .SingleOrDefaultAsync(x => x.Date == b.Date && x.Comment != null && x.Comment.Contains(b.AmountInLiters.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)));

        var entity = new Model.Database.Main.Transport.CarFuel()
        {
            AmountInLiters = b.AmountInLiters,
            CarId = carId,
            Expense = expense,
            Timestamp = b.Date
        };
        await context.CarFuelings.AddAsync(entity);
        await context.SaveChangesAsync();
    }
}
