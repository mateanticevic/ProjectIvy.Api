using ProjectIvy.Common.Extensions;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Model.Binding.Call;
using ProjectIvy.Model.Database.Main.User;
using System.Linq;

namespace ProjectIvy.Business.MapExtensions
{
    public static class CallExtensions
    {
        public static Call ToEntity(this CallBinding b, MainContext context, Call c = null)
        {
            c = c.DefaultIfNull();
            c.Duration = b.Duration;
            c.FileId = context.Files.SingleOrDefault(x => x.ValueId == b.FileId).Id;
            c.Number = b.Number;
            c.Timestamp = b.Timestamp;
            c.ValueId = b.Timestamp.ToString("yyyyMMddHHmmss");

            return c;
        }
    }
}
