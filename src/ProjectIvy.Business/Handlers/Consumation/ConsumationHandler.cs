using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
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

        public PagedView<CountBy<Model.View.Beer.Beer>> CountByBeer(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Consumations.WhereUser(User)
                                           .Where(binding, context)
                                           .Include(x => x.Beer)
                                           .GroupBy(x => x.Beer)
                                           .Select(x => new CountBy<Model.View.Beer.Beer>(x.Key.ConvertTo(y => new Model.View.Beer.Beer(y)), x.Count()))
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
                                           .Select(x => new View.Consumation.Consumation(x))
                                           .OrderByDescending(x => x.Date)
                                           .ToPagedView(binding);
            }
        }

        public PagedView<View.Beer.Beer> GetNewBeers(FilteredPagedBinding binding)
        {
            using (var context = GetMainContext())
            {
                var oldBeerIds = new List<int>();

                if (binding.From.HasValue)
                {
                    var filter = new FilteredPagedBinding()
                    {
                        To = binding.From.Value.AddDays(-1)
                    };

                    oldBeerIds.AddRange(context.Consumations.WhereUser(User)
                                               .Where(filter)
                                               .Select(x => x.Beer)
                                               .Distinct()
                                               .Select(x => x.Id));
                }

                return context.Consumations.WhereUser(User)
                                           .Include(x => x.Beer)
                                           .Where(binding)
                                           .GroupBy(x => x.Beer)
                                           .Select(x => new { Beer = x.Key, Date = x.Min(y => y.Date) })
                                           .Where(x => !oldBeerIds.Any(y => x.Beer.Id == y))
                                           .OrderByDescending(x => x.Date)
                                           .Select(x => new View.Beer.Beer(x.Beer))
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

        public PagedView<SumBy<Model.View.Beer.Beer>> SumVolumeByBeer(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var grouped = context.Consumations.WhereUser(User)
                                     .Where(binding, context)
                                     .Include(x => x.Beer)
                                     .GroupBy(x => x.Beer);

                return grouped.Select(x => new SumBy<Model.View.Beer.Beer>(x.Key.ConvertTo(y => new Model.View.Beer.Beer(y)), x.Sum(y => y.Volume)))
                              .OrderByDescending(x => x.Sum)
                              .ToPagedView(binding, grouped.Count());
            }
        }

        public PagedView<GroupedByMonth<int>> SumVolumeByMonth(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Consumations.WhereUser(User)
                                           .Where(binding, context)
                                           .GroupBy(x => new { x.Date.Year, x.Date.Month })
                                           .Select(x => new GroupedByMonth<int>(x.Sum(y => y.Volume), x.Key.Year, x.Key.Month))
                                           .OrderByDescending(x => x.Data)
                                           .ToPagedView(binding);
            }
        }

        public PagedView<SumBy<Model.View.Beer.BeerServing>> SumVolumeByServing(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var grouped = context.Consumations.WhereUser(User)
                                                  .Where(binding, context)
                                                  .Include(x => x.BeerServing)
                                                  .GroupBy(x => x.BeerServing);

                return grouped.Select(x => new SumBy<Model.View.Beer.BeerServing>(x.Key.ConvertTo(y => new Model.View.Beer.BeerServing(y)), x.Sum(y => y.Volume)))
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
