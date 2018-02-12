using Microsoft.EntityFrameworkCore;
using ProjectIvy.DL.Extensions.Entities;
using ProjectIvy.DL.Extensions;
using ProjectIvy.Model.Binding.Consumation;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Model.View;
using System.Linq;
using View = ProjectIvy.Model.View.Consumation;

namespace ProjectIvy.BL.Handlers.Consumation
{
    public class ConsumationHandler : Handler<ConsumationHandler>, IConsumationHandler
    {
        public ConsumationHandler(IHandlerContext<ConsumationHandler> context) : base(context)
        {
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
                return context.Consumations.WhereUser(User)
                                           .Where(binding, context)
                                           .Include(x => x.Beer)
                                           .GroupBy(x => x.Beer)
                                           .Select(x => new SumBy<Model.View.Beer.Beer>(x.Key.ConvertTo(y => new Model.View.Beer.Beer(y)), x.Sum(y => y.Volume)))
                                           .OrderByDescending(x => x.Sum)
                                           .ToPagedView(binding);
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
