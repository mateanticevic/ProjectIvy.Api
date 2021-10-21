﻿using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.Exceptions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Device;
using ProjectIvy.Model.Binding.Log;
using ProjectIvy.Model.Database.Main.Log;
using ProjectIvy.Model.Database.Main.Net;
using ProjectIvy.Model.View;
using System.Linq;
using View = ProjectIvy.Model.View;

namespace ProjectIvy.Business.Handlers.Device
{
    public class DeviceHandler : Handler<DeviceHandler>, IDeviceHandler
    {
        public DeviceHandler(IHandlerContext<DeviceHandler> context) : base(context)
        {
        }

        public void CreateBrowserLog(BrowserLogBinding binding)
        {
            try
            {
                using (var db = GetMainContext())
                {
                    int? deviceId = db.Devices.SingleOrDefault(x => x.ValueId == binding.DeviceId)?.Id;

                    if (!deviceId.HasValue)
                        throw new ResourceNotFoundException();

                    int? domainId = db.Domains.SingleOrDefault(x => x.ValueId == binding.Domain)?.Id;
                    if (!domainId.HasValue)
                    {
                        var webSite = new Model.Database.Main.Net.Web()
                        {
                            Name = binding.Domain,
                            ValueId = binding.Domain
                        };

                        db.Webs.Add(webSite);

                        var domain = new Domain()
                        {
                            ValueId = binding.Domain,
                            WebId = webSite.Id
                        };

                        db.Domains.Add(domain);

                        domainId = domain.Id;
                    }

                    var browserLog = new BrowserLog()
                    {
                        DeviceId = deviceId.Value,
                        DomainId = domainId.Value,
                        TimestampStart = binding.Start,
                        TimestampEnd = binding.End,
                        IsSecured = binding.IsSecured
                    };

                    db.BrowserLogs.Add(browserLog);

                    db.SaveChanges();
                }
            }
            catch (DbUpdateException)
            {
                throw new ResourceExistsException(nameof(BrowserLog));
            }
        }

        public PagedView<View.Device.BrowserLog> Get(LogBrowserGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                int? deviceId = context.Devices.GetId(binding.DomainId);

                var deviceIds = context.Devices.WhereUser(UserId).Select(x => x.Id).ToList();

                int? domainId = context.Domains.GetId(binding.DomainId);
                int? webId = context.Webs.GetId(binding.DomainId);

                var r = context.BrowserLogs.Include(x => x.Domain)
                                           .ThenInclude(x => x.Web)
                                           .Where(x => deviceIds.Contains(x.DeviceId))
                                           .WhereIf(domainId.HasValue, x => x.DomainId == domainId.Value)
                                           .WhereIf(webId.HasValue, x => x.Domain.WebId == webId.Value)
                                           .WhereIf(binding.From.HasValue, x => x.TimestampStart >= binding.From.Value)
                                           .WhereIf(binding.To.HasValue, x => x.TimestampEnd <= binding.To.Value);

                var items = r.Page(binding).ToList().Select(x => new View.Device.BrowserLog(x));

                long count = r.Count();

                return new PagedView<View.Device.BrowserLog>(items, count);
            }
        }
    }
}
