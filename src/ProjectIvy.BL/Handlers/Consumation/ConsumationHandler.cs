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

        public int VolumeSum(ConsumationGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Consumations.WhereUser(User)
                                           .WhereIf(binding.From.HasValue, x => x.Date >= binding.From.Value)
                                           .WhereIf(binding.To.HasValue, x => x.Date <= binding.To.Value)
                                           .Sum(x => x.Volume);
            }
        }
    }
}
