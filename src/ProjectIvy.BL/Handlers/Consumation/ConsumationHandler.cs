using ProjectIvy.DL.Extensions.Entities;
using ProjectIvy.DL.Extensions;
using ProjectIvy.Model.Binding.Consumation;
using System.Linq;

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
                              .Where(binding)
                              .Count();
            }
        }

        public int VolumeSum(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Consumations.WhereUser(User)
                                           .Where(binding)
                                           .Sum(x => x.Volume);
            }
        }
    }
}
