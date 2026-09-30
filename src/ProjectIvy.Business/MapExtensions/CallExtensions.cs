using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Model.Binding.Call;
using ProjectIvy.Model.Database.Main.Contacts;

namespace ProjectIvy.Business.MapExtensions;

public static class CallExtensions
{
    public static async Task<Call> ToEntity(this CallBinding b, MainContext context, Call c = null)
    {
        c = c.DefaultIfNull();
        c.Duration = b.Duration;
        c.FileId = (await context.Files.SingleOrDefaultAsync(x => x.ValueId == b.FileId)).Id;
        c.Number = b.Number;
        c.Timestamp = b.Timestamp;
        c.ValueId = b.Timestamp.ToString("yyyyMMddHHmmss");

        return c;
    }
}
