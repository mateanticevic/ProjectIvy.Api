using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Consumation;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using System.Linq;
using System;
using View = ProjectIvy.Model.View.Consumation;

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

        public int CountUniqueBeers(ConsumationGetBinding binding)
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

        public int CountUniqueBrands(ConsumationGetBinding binding)
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

        public PagedView<View.Consumation> Get(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Consumations.WhereUser(User)
                                           .Where(binding, context)
                                           .Select(x => new View.Consumation(x))
                                           .OrderByDescending(x => x.Date)
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
