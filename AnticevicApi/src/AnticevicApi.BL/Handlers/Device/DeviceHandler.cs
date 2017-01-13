using AnticevicApi.Model.Binding.Device;
using AnticevicApi.Model.Database.Main.Log;
using AnticevicApi.Model.Database.Main.Net;
using System.Linq;

namespace AnticevicApi.BL.Handlers.Device
{
    public class DeviceHandler : Handler<DeviceHandler>, IDeviceHandler
    {
        public DeviceHandler(IHandlerContext<DeviceHandler> context) : base(context)
        {

        }

        public bool CreateBrowserLog(BrowserLogBinding binding)
        {
            using (var db = GetMainContext())
            {
                int deviceId = db.Devices.SingleOrDefault(x => x.ValueId == binding.DeviceId).Id;

                int? domainId = db.Domains.SingleOrDefault(x => x.ValueId == binding.Domain)?.Id;
                if(!domainId.HasValue)
                {
                    var webSite = new WebSite()
                    {
                        Name = binding.Domain,
                        ValueId = binding.Domain
                    };

                    db.WebSites.Add(webSite);

                    var domain = new Domain()
                    {
                        ValueId = binding.Domain,
                        WebSiteId = webSite.Id
                    };

                    db.Domains.Add(domain);

                    domainId = domain.Id;
                }

                var browserLog = new BrowserLog()
                {
                    DeviceId = deviceId,
                    DomainId = domainId.Value,
                    TimestampStart = binding.Start,
                    TimestampEnd = binding.End,
                    IsSecured = binding.IsSecured
                };

                db.BrowserLogs.Add(browserLog);

                db.SaveChanges();

                return true;
            }
        }
    }
}
