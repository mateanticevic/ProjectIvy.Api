using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.IotData;

namespace ProjectIvy.Business.Handlers.IotDevice
{
    public class IotDeviceHandler : Handler<IotDeviceHandler>, IIotDeviceHandler
    {
        public IotDeviceHandler(IHandlerContext<IotDeviceHandler> context) : base(context)
        {
        }

        public async Task Ping(string deviceId)
        {
            using (var context = GetMainContext())
            {
                var entity = await context.IotDevices.WhereUser(User.Id)
                                                     .SingleAsync(x => x.ValueId == deviceId);
                entity.LastPing = DateTime.Now;
                await context.SaveChangesAsync();
            }
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
