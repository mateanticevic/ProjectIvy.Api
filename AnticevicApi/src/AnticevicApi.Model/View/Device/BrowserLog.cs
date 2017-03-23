using AnticevicApi.Extensions.BuiltInTypes;
using DatabaseModel = AnticevicApi.Model.Database.Main;
using System;

namespace AnticevicApi.Model.View.Device
{
    public class BrowserLog
    {
        public BrowserLog(DatabaseModel.Log.BrowserLog log)
        {
            Domain = log.Domain.ConvertTo(x => new Domain.Domain(x));
            End = log.TimestampEnd;
            Start = log.TimestampStart;
            Web = log.Domain.Web.ConvertTo(x => new Web.Web(x));
        }

        public Domain.Domain Domain { get; set; }

        public DateTime End { get; set; }

        public DateTime Start { get; set; }

        public Web.Web Web { get; set; }
    }
}
