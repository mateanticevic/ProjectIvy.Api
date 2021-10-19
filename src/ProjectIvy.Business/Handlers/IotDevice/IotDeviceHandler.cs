using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<KeyValuePair<DateTime, string>>> GetData(string deviceId, string fieldIdentifier, IotDeviceDataGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                int id = context.IotDevices.WhereUser(UserId.Value).GetId(deviceId).Value;

                return await context.IotDeviceData.Where(x => x.DeviceId == id && x.FieldIdentifier == fieldIdentifier)
                                                  .WhereCreatedInclusive(binding)
                                                  .OrderByDescending(x => x.Created)
                                                  .Select(x => new KeyValuePair<DateTime, string>(x.Created, x.Value))
                                                  .ToListAsync();
            }
        }

        public async Task Ping(string deviceId)
        {
            using (var context = GetMainContext())
            {
                var entity = await context.IotDevices.WhereUser(UserId.Value)
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
