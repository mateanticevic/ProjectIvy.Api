﻿using Dapper;
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
using System;
using System.Collections.Generic;
using System.Linq;
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
                    consumation.UserId = User.Id;

                    context.Consumations.Add(consumation);
                }

                context.SaveChanges();
            }
        }

        public IEnumerable<(DateTime From, DateTime To)> ConsecutiveDates(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Consumations.WhereUser(User)
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
                return context.Consumations.WhereUser(User)
                              .Where(binding, context)
                              .Count();
            }
        }

        public PagedView<CountBy<View.Beer.Beer>> CountByBeer(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Consumations.WhereUser(User)
                                           .Where(binding, context)
                                           .Include(x => x.Beer)
                                           .GroupBy(x => x.Beer)
                                           .Select(x => new CountBy<View.Beer.Beer>(x.Key.ConvertTo(y => new View.Beer.Beer(y)), x.Count()))
                                           .OrderByDescending(x => x.Count)
                                           .ToPagedView(binding);
            }
        }

        public IEnumerable<KeyValuePair<string, int>> CountByMonth(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var to = binding.To ?? DateTime.Now;

                return context.Consumations.WhereUser(User.Id)
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

                return context.Consumations.WhereUser(User.Id)
                                           .Where(binding, context)
                                           .GroupBy(x => new { x.Date.Year, x.Date.Month })
                                           .Select(x => new GroupedByMonth<int>(x.Count(), x.Key.Year, x.Key.Month))
                                           .ToList()
                                           .FillMissingMonths(datetime => new GroupedByMonth<int>(0, datetime.Year, datetime.Month), binding.From, to)
                                           .Select(x => new KeyValuePair<string, int>($"{x.Year}-{x.Month}", x.Data));
            }
        }

        public IEnumerable<KeyValuePair<string, int>> CountByYear(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var to = binding.To ?? DateTime.Now;

                return context.Consumations.WhereUser(User.Id)
                                           .Where(binding, context)
                                           .GroupBy(x => x.Date.Year)
                                           .Select(x => new GroupedByYear<int>(x.Count(), x.Key))
                                           .ToList()
                                           .FillMissingYears(year => new GroupedByYear<int>(0, year), binding.From?.Year, to.Year)
                                           .Select(x => new KeyValuePair<string, int>(x.Year.ToString(), x.Data));
            }
        }

        public int CountBeers(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Consumations.WhereUser(User)
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
                return context.Consumations.WhereUser(User)
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
                return context.Consumations.WhereUser(User)
                                           .Where(binding, context)
                                           .OrderByDescending(x => x.Date)
                                           .Select(x => new View.Consumation.Consumation(x))
                                           .ToPagedView(binding);
            }
        }

        public PagedView<View.Beer.Beer> GetNewBeers(FilteredPagedBinding binding)
        {
            using (var context = GetMainContext())
            {
                var oldBeerIds = binding.From.HasValue ? context.Consumations.WhereUser(User)
                                               .Where(new FilteredPagedBinding()
                                               {
                                                   To = binding.From.Value.AddDays(-1)
                                               })
                                               .Select(x => x.Beer.Id) : null;

                return context.Consumations.WhereUser(User)
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
                return context.Consumations.WhereUser(User)
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
                return context.Consumations.WhereUser(User)
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
                                     .WhereUser(User)
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

        public PagedView<KeyValuePair<View.Country.Country, int>> SumVolumeByCountry(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var grouped = context.Consumations
                                     .WhereUser(User)
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

        public IEnumerable<KeyValuePair<int, int>> SumVolumeByDayOfWeek(ConsumationGetBinding binding)
        {
            using (var sqlConnection = GetSqlConnection())
            {
                return sqlConnection.Query<KeyValuePair<int, int>>(SqlLoader.Load(Constants.GetConsumationSumByDayOfWeek),
                                        new {
                                            binding.From,
                                            binding.To,
                                            UserId = User.Id
                                        })
                                    .Select(x => new KeyValuePair<int, int>(x.Key == 1 ? 7 : x.Key - 1, x.Value))
                                    .OrderBy(x => x.Key);
            }
        }

        public IEnumerable<GroupedByMonth<int>> SumVolumeByMonthOfYear(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Consumations.WhereUser(User)
                                           .Where(binding, context)
                                           .GroupBy(x => new { x.Date.Year, x.Date.Month })
                                           .Select(x => new GroupedByMonth<int>(x.Sum(y => y.Volume), x.Key.Year, x.Key.Month))
                                           .ToList()
                                           .OrderBy(x => x.Year)
                                           .ThenBy(x => x.Month);
            }
        }

        public IEnumerable<KeyValuePair<int, int>> SumVolumeByYear(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Consumations.WhereUser(User)
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
                var grouped = context.Consumations.WhereUser(User)
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

        public PagedView<SumBy<View.Beer.BeerStyle>> SumVolumeByStyle(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var grouped = context.Consumations
                                     .WhereUser(User)
                                     .Where(binding, context)
                                     .Include(x => x.Beer)
                                     .ThenInclude(x => x.BeerStyle)
                                     .Select(x => new { x.Beer.BeerStyle, x.Volume })
                                     .GroupBy(x => x.BeerStyle);

                return grouped.Select(x => new SumBy<View.Beer.BeerStyle>(x.Key.ConvertTo(y => new View.Beer.BeerStyle(y)), x.Sum(y => y.Volume)))
                              .OrderByDescending(x => x.Sum)
                              .ToPagedView(binding, grouped.Count());
            }
        }

        public int SumVolume(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Consumations.WhereUser(User)
                                           .Where(binding, context)
                                           .Sum(x => x.Volume);
            }
        }
    }
}
