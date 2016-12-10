using AnticevicApi.Model.Database.Main.Finance;
using AnticevicApi.Model.Database.Main.Transport;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AnticevicApi.DL.Extensions
{
    public static class DbSetExtensions
    {
        public static int? GetId(this DbSet<Car> set, string valueId)
        {
            return set.SingleOrDefault(x => x.ValueId == valueId)?.Id;
        }

        public static int? GetId(this DbSet<ExpenseType> set, string valueId)
        {
            return set.SingleOrDefault(x => x.ValueId == valueId)?.Id;
        }

        public static int? GetId(this DbSet<Vendor> set, string valueId)
        {
            return set.SingleOrDefault(x => x.ValueId == valueId)?.Id;
        }
    }
}
