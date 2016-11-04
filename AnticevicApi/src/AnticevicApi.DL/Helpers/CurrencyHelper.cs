using AnticevicApi.DL.DbContexts;
using System.Linq;

namespace AnticevicApi.DL.Helpers
{
    public class CurrencyHelper
    {
        public static int GetId(string code)
        {
            using (var db = new MainContext())
            {
                return db.Currencies.SingleOrDefault(x => x.Code == code)
                                    .Id;
            }
        }
    }
}
