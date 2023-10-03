using Dapper;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Data.Sql;
using ProjectIvy.Data.Sql.Main.Scripts;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Consumation;
using ProjectIvy.Model.View;
using System.Linq;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View;

namespace ProjectIvy.Business.Handlers.Consumation
{
    public class ConsumationHandler : Handler<ConsumationHandler>, IConsumationHandler
    {
        public ConsumationHandler(IHandlerContext<ConsumationHandler> context) : base(context)
        {
        }

        public void Add(ConsumationBinding binding)
        {
            using (var context = GetMainContext())
            {
                foreach (var i in Enumerable.Range(0, binding.Units))
                {
                    var consumation = binding.ToEntity(context);
                    consumation.UserId = UserId;

                    context.Consumations.Add(consumation);
                }

                context.SaveChanges();
            }
        }

        public async Task<IEnumerable<KeyValuePair<int, decimal>>> AlcoholByYear(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return await context.Consumations.WhereUser(UserId)
                                                 .Where(binding, context)
                                                 .GroupBy(x => x.Date.Year)
                                                 .OrderBy(x => x.Key)
                                                 .Select(x => new KeyValuePair<int, decimal>(x.Key, x.Sum(y => y.Volume * y.Beer.Abv / 100)))
                                                 .ToListAsync();
            }
        }

        public async Task<IEnumerable<KeyValuePair<int, int>>> AverageByYear(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                //TODO: Take leap years into account

                var sumByYear = await context.Consumations.WhereUser(UserId)
                                                 .Where(binding, context)
                                                 .GroupBy(x => x.Date.Year)
                                                 .OrderBy(x => x.Key)
                                                 .Select(x => new KeyValuePair<int, int>(x.Key, x.Sum(y => y.Volume)))
                                                 .ToListAsync();

                return sumByYear.Select(x => new KeyValuePair<int, int>(x.Key, x.Key == DateTime.Now.Year ? (int)(x.Value / DateTime.Now.Subtract(new DateTime(DateTime.Now.Year, 1, 1)).TotalDays) : x.Value / 365));
            }
        }

        public IEnumerable<(DateTime From, DateTime To)> ConsecutiveDates(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Consumations.WhereUser(UserId)
                                           .Where(binding, context)
                                           .Select(x => x.Date)
                                           .Distinct()
                                           .ToList()
                                           .ConsecutiveDates()
                                           .Select(x => new { Range = x, Count = x.To.Subtract(x.From).Days + 1 })
                                           .OrderByDescending(x => x.Count)
                                           .Select(x => x.Range)
                                           .ToList();
            }
        }

        public int Count(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Consumations.WhereUser(UserId)
                              .Where(binding, context)
                              .Count();
            }
        }

        public PagedView<KeyValuePair<View.Beer.Beer, int>> CountByBeer(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Consumations.WhereUser(UserId)
                                           .Where(binding, context)
                                           .Include(x => x.Beer)
                                           .GroupBy(x => new
                                           {
                                               x.Beer.ValueId,
                                               x.Beer.Name
                                           })
                                           .OrderByDescending(x => x.Count())
                                           .Select(x => new KeyValuePair<View.Beer.Beer, int>(new View.Beer.Beer()
                                           {
                                               Id = x.Key.ValueId,
                                               Name = x.Key.Name
                                           }, x.Count()))
                                           .ToPagedView(binding);
            }
        }

        public IEnumerable<KeyValuePair<string, int>> CountByMonth(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var to = binding.To ?? DateTime.Now;

                return context.Consumations.WhereUser(UserId)
                                           .Where(binding, context)
                                           .GroupBy(x => x.Date.ToString("MMMM"))
                                           .Select(x => new KeyValuePair<string, int>(x.Key, x.Count()))
                                           .ToList();
            }
        }

        public IEnumerable<KeyValuePair<string, int>> CountByMonthOfYear(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var to = binding.To ?? DateTime.Now;

                return context.Consumations.WhereUser(UserId)
                                           .Where(binding, context)
                                           .GroupBy(x => new { x.Date.Year, x.Date.Month })
                                           .Select(x => new GroupedByMonth<int>(x.Count(), x.Key.Year, x.Key.Month))
                                           .ToList()
                                           .FillMissingMonths(datetime => new GroupedByMonth<int>(0, datetime.Year, datetime.Month), binding.From, to)
                                           .Select(x => new KeyValuePair<string, int>($"{x.Year}-{x.Month}", x.Data));
            }
        }

        public IEnumerable<KeyValuePair<int, int>> CountByYear(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var to = binding.To ?? DateTime.Now;

                return context.Consumations.WhereUser(UserId)
                                           .Where(binding, context)
                                           .GroupBy(x => x.Date.Year)
                                           .Select(x => new KeyValuePair<int, int>(x.Count(), x.Key))
                                           .ToList()
                                           .FillMissingYears(year => new KeyValuePair<int, int>(0, year), binding.From?.Year, to.Year);
            }
        }

        public int CountBeers(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Consumations.WhereUser(UserId)
                              .Where(binding, context)
                              .Select(x => x.BeerId)
                              .Distinct()
                              .Count();
            }
        }

        public int CountBrands(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Consumations.WhereUser(UserId)
                                           .Where(binding, context)
                                           .Include(x => x.Beer)
                                           .Select(x => x.Beer.BeerBrandId)
                                           .Distinct()
                                           .Count();
            }
        }

        public PagedView<View.Consumation.Consumation> Get(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Consumations.WhereUser(UserId)
                                           .Where(binding, context)
                                           .Include(x => x.Beer)
                                           .ThenInclude(x => x.BeerStyle)
                                           .Include(x => x.Beer)
                                           .ThenInclude(x => x.BeerBrand)
                                           .OrderByDescending(x => x.Date)
                                           .Select(x => new View.Consumation.Consumation(x))
                                           .ToPagedView(binding);
            }
        }

        public async Task<IEnumerable<View.Country.Country>> GetCountries(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return await context.Consumations
                                    .WhereUser(UserId)
                                    .Where(binding, context)
                                    .Include(x => x.Beer)
                                    .ThenInclude(x => x.BeerBrand)
                                    .ThenInclude(x => x.Country)
                                    .Select(x => x.Beer.BeerBrand.Country)
                                    .Where(x => x != null)
                                    .Distinct()
                                    .Select(x => new View.Country.Country(x))
                                    .ToListAsync();
            }
        }

        public PagedView<View.Beer.Beer> GetNewBeers(FilteredPagedBinding binding)
        {
            using (var context = GetMainContext())
            {
                var oldBeerIds = binding.From.HasValue ? context.Consumations.WhereUser(UserId)
                                               .Where(new FilteredPagedBinding()
                                               {
                                                   To = binding.From.Value.AddDays(-1)
                                               })
                                               .Select(x => x.Beer.Id) : null;

                return context.Consumations.WhereUser(UserId)
                                           .Include(x => x.Beer)
                                           .Where(binding)
                                           .GroupBy(x => new
                                           {
                                               x.Beer.Id,
                                               x.Beer.Name,
                                               x.Beer.ValueId
                                           })
                                           .Select(x => new { Beer = x.Key, Date = x.Min(y => y.Date) })
                                           .Where(x => oldBeerIds == null || !oldBeerIds.Any(y => x.Beer.Id == y))
                                           .OrderByDescending(x => x.Date)
                                           .Select(x => new View.Beer.Beer()
                                           {
                                               Id = x.Beer.ValueId,
                                               Name = x.Beer.Name
                                           })
                                           .ToPagedView(binding);
            }
        }

        public PagedView<View.Beer.Beer> GetBeers(FilteredPagedBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Consumations.WhereUser(UserId)
                                           .Where(binding)
                                           .Select(x => x.Beer)
                                           .Distinct()
                                           .Select(x => new View.Beer.Beer(x))
                                           .ToPagedView(binding);
            }
        }

        public PagedView<View.Beer.BeerBrand> GetBrands(FilteredPagedBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Consumations.WhereUser(UserId)
                                           .Where(binding)
                                           .Select(x => x.Beer.BeerBrand)
                                           .Distinct()
                                           .Select(x => new View.Beer.BeerBrand(x))
                                           .ToPagedView(binding);
            }
        }

        public PagedView<KeyValuePair<View.Beer.Beer, int>> SumVolumeByBeer(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var grouped = context.Consumations
                                     .WhereUser(UserId)
                                     .Where(binding, context)
                                     .Include(x => x.Beer)
                                     .GroupBy(x => new
                                     {
                                         x.Beer.Name,
                                         x.Beer.ValueId
                                     });

                return grouped.OrderByDescending(x => x.Sum(y => y.Volume))
                              .Select(x => new KeyValuePair<View.Beer.Beer, int>(new()
                              {
                                  Id = x.Key.ValueId,
                                  Name = x.Key.Name
                              }, x.Sum(y => y.Volume)))
                              .ToPagedView(binding, grouped.Count());
            }
        }

        public PagedView<KeyValuePair<View.Beer.BeerBrand, int>> SumVolumeByBrand(ConsumationGetBinding binding)
        {
            using var context = GetMainContext();

            var grouped = context.Consumations
                          .WhereUser(UserId)
                          .Where(binding, context)
                          .Include(x => x.Beer)
                          .ThenInclude(x => x.BeerBrand)
                          .GroupBy(x => new
                          {
                              x.Beer.BeerBrand.Name,
                              x.Beer.BeerBrand.ValueId
                          });

            return grouped.OrderByDescending(x => x.Sum(y => y.Volume))
                          .Select(x => new KeyValuePair<View.Beer.BeerBrand, int>(new()
                          {
                              Id = x.Key.ValueId,
                              Name = x.Key.Name
                          }, x.Sum(y => y.Volume)))
                          .ToPagedView(binding, grouped.Count());

        }

        public PagedView<KeyValuePair<View.Country.Country, int>> SumVolumeByCountry(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var grouped = context.Consumations
                                     .WhereUser(UserId)
                                     .Where(binding, context)
                                     .Include(x => x.Beer)
                                     .ThenInclude(x => x.BeerBrand)
                                     .ThenInclude(x => x.Country)
                                     .Select(x => new { x.Beer.BeerBrand.Country, x.Volume })
                                     .GroupBy(x => new
                                     {
                                         x.Country.Name,
                                         x.Country.ValueId
                                     });

                return grouped.OrderByDescending(x => x.Sum(y => y.Volume))
                              .Select(x => new KeyValuePair<View.Country.Country, int>(new()
                              {
                                  Id = x.Key.ValueId,
                                  Name = x.Key.Name
                              }, x.Sum(y => y.Volume)))
                              .ToPagedView(binding, grouped.Count());
            }
        }

        public IEnumerable<KeyValuePair<DateTime, int>> SumVolumeByDay(ConsumationGetBinding binding)
        {
            using var context = GetMainContext();

            return context.Consumations.WhereUser(UserId)
                           .Where(binding, context)
                           .GroupBy(x => x.Date)
                           .Select(x => new KeyValuePair<DateTime, int>(x.Key, x.Sum(y => y.Volume)))
                           .ToList()
                           .OrderBy(x => x.Key);
        }

        public IEnumerable<KeyValuePair<int, int>> SumVolumeByDayOfWeek(ConsumationGetBinding binding)
        {
            using (var sqlConnection = GetSqlConnection())
            {
                return sqlConnection.Query<KeyValuePair<int, int>>(SqlLoader.Load(SqlScripts.GetConsumationSumByDayOfWeek),
                                        new
                                        {
                                            binding.From,
                                            binding.To,
                                            UserId = UserId
                                        })
                                    .Select(x => new KeyValuePair<int, int>(x.Key == 1 ? 6 : x.Key - 2, x.Value))
                                    .OrderBy(x => x.Key);
            }
        }

        public IEnumerable<KeyValuePair<int, int>> SumVolumeByMonth(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Consumations.WhereUser(UserId)
                                           .Where(binding, context)
                                           .GroupBy(x => x.Date.Month)
                                           .Select(x => new KeyValuePair<int, int>(x.Key, x.Sum(y => y.Volume)))
                                           .ToList()
                                           .FillMissingMonths()
                                           .OrderBy(x => x.Key);
            }
        }

        public IEnumerable<KeyValuePair<DateTime, int>> SumVolumeByMonthOfYear(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Consumations.WhereUser(UserId)
                                           .Where(binding, context)
                                           .GroupBy(x => new { x.Date.Year, x.Date.Month })
                                           .Select(x => new KeyValuePair<DateTime, int>(new DateTime(x.Key.Year, x.Key.Month, 1), x.Sum(y => y.Volume)))
                                           .ToList()
                                           .OrderBy(x => x.Key);
            }
        }

        public IEnumerable<KeyValuePair<int, int>> SumVolumeByYear(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Consumations.WhereUser(UserId)
                                           .Where(binding, context)
                                           .GroupBy(x => x.Date.Year)
                                           .Select(x => new KeyValuePair<int, int>(x.Key, x.Sum(y => y.Volume)))
                                           .ToList()
                                           .OrderBy(x => x.Key);
            }
        }

        public IEnumerable<KeyValuePair<View.Beer.BeerServing, int>> SumVolumeByServing(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var grouped = context.Consumations.WhereUser(UserId)
                                                  .Where(binding, context)
                                                  .Include(x => x.BeerServing)
                                                  .GroupBy(x => new
                                                  {
                                                      x.BeerServing.Name,
                                                      x.BeerServing.ValueId
                                                  });

                return grouped.OrderByDescending(x => x.Sum(y => y.Volume))
                              .Select(x => new KeyValuePair<View.Beer.BeerServing, int>(new()
                              {
                                  Id = x.Key.ValueId,
                                  Name = x.Key.Name
                              }, x.Sum(y => y.Volume)))
                                              .ToList();
            }
        }

        public PagedView<KeyValuePair<View.Beer.BeerStyle, int>> SumVolumeByStyle(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var grouped = context.Consumations
                                     .WhereUser(UserId)
                                     .Where(binding, context)
                                     .Include(x => x.Beer)
                                     .ThenInclude(x => x.BeerStyle)
                                     .GroupBy(x => new
                                     {
                                         x.Beer.BeerStyle.ValueId,
                                         x.Beer.BeerStyle.Name
                                     });

                return grouped.OrderByDescending(x => x.Sum(y => y.Volume))
                              .Select(x => new KeyValuePair<View.Beer.BeerStyle, int>(
                                  new()
                                  {
                                      Id = x.Key.ValueId,
                                      Name = x.Key.Name
                                  }, x.Sum(y => y.Volume)))
                              .ToPagedView(binding, grouped.Count());
            }
        }

        public int SumVolume(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Consumations.WhereUser(UserId)
                                           .Where(binding, context)
                                           .Sum(x => x.Volume);
            }
        }
    }
}
