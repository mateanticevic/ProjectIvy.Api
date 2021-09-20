using System.Threading.Tasks;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Model.Binding.IotData;

namespace ProjectIvy.Business.Handlers.IotDevice
{
    public class IotDeviceHandler : Handler<IotDeviceHandler>, IIotDeviceHandler
    {
        public IotDeviceHandler(IHandlerContext<IotDeviceHandler> context) : base(context)
        {
        }

        public async Task PushData(IotDeviceDataBinding b)
        {
            using (var context = GetMainContext())
            {
                var entity = b.ToEntity(context);
                await context.IotDeviceData.AddAsync(entity);
                await context.SaveChangesAsync();
            }
        }
    }
}
