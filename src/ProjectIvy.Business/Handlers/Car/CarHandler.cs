using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.Exceptions;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Model.Binding.Car;
using ProjectIvy.Model.View.Car;
using System.Linq;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.Car;

namespace ProjectIvy.Business.Handlers.Car
{
    public class CarHandler : Handler<CarHandler>, ICarHandler
    {
        public CarHandler(IHandlerContext<CarHandler> context) : base(context) {}

        public void Create(string valueId, CarBinding car)
        {
            using (var context = GetMainContext())
            {
                var entity = car.ToEntity(context);
                entity.ValueId = valueId;
                entity.UserId = UserId;

                context.Cars.Add(entity);
                context.SaveChanges();
            }
        }

        public DateTime CreateLog(CarLogBinding binding)
        {
            using (var context = GetMainContext())
            {
                if (string.IsNullOrWhiteSpace(binding.CarValueId))
                    binding.CarValueId = context.Users.Include(x => x.DefaultCar).SingleOrDefault(x => x.Id == UserId).DefaultCar.ValueId;

                var lastEntry = GetLatestLog(binding.CarValueId, new CarLogGetBinding() { HasOdometer = true });

                if (lastEntry != null && binding.Odometer < lastEntry.Odometer)
                {
                    throw new InvalidRequestException($"Odometer must be {lastEntry.Odometer}km or higher.");
                }

                var entity = binding.ToEntity(context);

                context.CarLogs.Add(entity);
                context.SaveChanges();

                return entity.Timestamp;
            }
        }

        public async Task<string> CreateService(string carValueId, CarServiceBinding binding)
        {
            using (var context = GetMainContext())
            {
                var entity = binding.ToEntity(context);
                entity.CarId = context.Cars.GetId(carValueId).Value;

                context.CarServices.Add(entity);
                await context.SaveChangesAsync();

                return entity.ValueId;
            }
        }

        public void CreateTorqueLog(string carValueId, CarLogTorqueBinding binding)
        {
            using (var context = GetMainContext())
            {
                int? carId = context.Cars.GetId(carValueId);

                var entity = binding.ToEntity();
                entity.CarId = carId.Value;
                context.CarLogs.Add(entity);
                context.SaveChanges();
            }
        }

        public IEnumerable<View.Car> Get()
        {
            using (var context = GetMainContext())
            {
                return context.Cars.WhereUser(UserId)
                                   .Include(x => x.CarModel)
                                   .ThenInclude(x => x.Manufacturer)
                                   .ToList()
                                   .Select(x => new View.Car(x))
                                   .ToList();
            }
        }

        public View.Car Get(string carId)
        {
            using (var context = GetMainContext())
            {
                var car = context.Cars.WhereUser(UserId)
                                      .Include(x => x.CarServices)
                                      .Include($"{nameof(Model.Database.Main.Transport.Car.CarServices)}.{nameof(Model.Database.Main.Transport.CarServiceType)}")
                                      .Include(x => x.CarModel)
                                      .ThenInclude(x => x.Manufacturer)
                                      .SingleOrDefault(x => x.ValueId == carId);

                var lastLog = context.CarLogs.Where(x => x.Odometer.HasValue && x.CarId == car.Id)
                                             .OrderByDescending(x => x.Odometer)
                                             .FirstOrDefault();

                int averageKmPerDay = lastLog.Odometer.Value / lastLog.Timestamp.Subtract(car.FirstRegistered.Value).Days;

                var serviceDue = new List<View.CarServiceDue>();
                foreach (var serviceInterval in context.CarServiceIntervals
                                                       .Include(x => x.CarServiceType)
                                                       .Where(x => x.CarModelId == car.CarModelId && (x.Days.HasValue || x.Range.HasValue))
                                                       .ToList())
                {
                    var lastService = car.CarServices.Where(x => x.CarServiceTypeId == serviceInterval.CarServiceTypeId)
                                                     .OrderByDescending(x => x.Date)
                                                     .FirstOrDefault();

                    if (lastService == null && car.FirstRegistered.HasValue)
                    {
                        int? dueIn = serviceInterval.Range.HasValue ? serviceInterval.Range - lastLog.Odometer : null;

                        serviceDue.Add(new View.CarServiceDue
                        {
                            DueAt = serviceInterval.Range.HasValue ? serviceInterval.Range : null,
                            DueIn = dueIn,
                            DueBefore = serviceInterval.Days.HasValue ? car.FirstRegistered.Value.AddDays(serviceInterval.Days.Value) : null,
                            DueBeforeApprox = dueIn.HasValue ? DateTime.Now.AddDays(dueIn.Value / averageKmPerDay) : null,
                            ServiceType = new View.CarServiceType(serviceInterval.CarServiceType)
                        });
                    }
                    else
                    {
                        int? aproximateOdometer = context.CarLogs.GetAproximateOdometer(car.Id, lastService.Date);
                        int? dueIn = serviceInterval.Range.HasValue ? aproximateOdometer + serviceInterval.Range - lastLog.Odometer : null;

                        serviceDue.Add(new View.CarServiceDue()
                        {
                            DueAt = serviceInterval.Range.HasValue ? aproximateOdometer + serviceInterval.Range.Value : null,
                            DueIn = dueIn,
                            DueBefore = serviceInterval.Days.HasValue ? lastService.Date.AddDays(serviceInterval.Days.Value) : null,
                            DueBeforeApprox = dueIn.HasValue ? DateTime.Now.AddDays(dueIn.Value / averageKmPerDay) : null,
                            ServiceType = new View.CarServiceType(lastService.CarServiceType)
                        });
                    }
                }

                return new View.Car(car) { ServiceDue = serviceDue };
            }
        }

        public async Task<IEnumerable<CarFueling>> GetFuelings(string carValueId)
        {
            using var context = GetMainContext();

            return context.Cars.WhereUser(UserId)
                               .Include(x => x.CarFuelings)
                               .Single(x => x.ValueId == carValueId)
                               .CarFuelings
                               .OrderByDescending(x => x.Timestamp)
                               .Select(x => new CarFueling(x))
                               .ToList();
        }

        public IEnumerable<View.CarLogBySession> GetLogBySession(string carValueId, CarLogGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Cars.WhereUser(UserId)
                                   .Include(x => x.CarLogs)
                                   .SingleOrDefault(x => x.ValueId == carValueId)
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

        }

        public int GetLogCount(string carValueId)
        {
            using (var db = GetMainContext())
            {
                return db.Cars.WhereUser(UserId)
                                    .Include(x => x.CarLogs)
                                    .SingleOrDefault(x => x.ValueId == carValueId)
                                    .CarLogs
                                    .Count;
            }
        }

        public async Task<IEnumerable<View.CarLog>> GetLogs(string carValueId, CarLogGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                int? carId = context.Cars.GetId(carValueId);

                return await context.CarLogs
                                    .Where(x => x.CarId == carId.Value)
                                    .Where(binding)
                                    .OrderBy(x => x.Timestamp)
                                    .Select(x => new View.CarLog(x))
                                    .ToListAsync();
            }
        }

        public View.CarLog GetLatestLog(CarLogGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                string carValueId = context.Users.Include(x => x.DefaultCar).SingleOrDefault(x => x.Id == UserId).DefaultCar.ValueId;
                return GetLatestLog(carValueId, binding);
            }
        }

        public View.CarLog GetLatestLog(string carValueId, CarLogGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                int? carId = db.Cars.GetId(carValueId);

                //TODO: check if car belongs to user

                var carLog = db.CarLogs
                                    .Where(x => x.CarId == carId)
                                    .WhereIf(binding.HasOdometer.HasValue, x => x.Odometer.HasValue == binding.HasOdometer.Value)
                                    .OrderByDescending(x => x.Timestamp)
                                    .FirstOrDefault();

                return carLog == null ? null : new View.CarLog(carLog);
            }
        }

        public async Task<IEnumerable<View.CarServiceInterval>> GetServiceIntervals(string carModelValueId)
        {
            using (var context = GetMainContext())
            {
                int? carModelId = context.CarModels.GetId(carModelValueId);

                return await context.CarServiceIntervals.Where(x => x.CarModelId == carModelId)
                                                        .Include(x => x.CarServiceType)
                                                        .Select(x => new View.CarServiceInterval(x))
                                                        .ToListAsync();
            }
        }

        public async Task<IEnumerable<View.CarServiceType>> GetServiceTypes(string carModelValueId)
        {
            using (var context = GetMainContext())
            {
                int? carModelId = context.CarModels.GetId(carModelValueId);

                return await context.CarServiceIntervals.Where(x => x.CarModelId == carModelId)
                                                        .Include(x => x.CarServiceType)
                                                        .OrderBy(x => x.CarServiceType.Name)
                                                        .Select(x => new View.CarServiceType(x.CarServiceType))
                                                        .ToListAsync();
            }
        }
    }
}
