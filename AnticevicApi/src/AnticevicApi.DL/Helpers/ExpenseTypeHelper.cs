using AnticevicApi.DL.DbContexts;
using System.Linq;

namespace AnticevicApi.DL.Helpers
{
    public class ExpenseTypeHelper
    {
        public static int? GetId(string valueId)
        {
            using (var db = new MainContext(""))
            {
                return db.ExpenseTypes.SingleOrDefault(x => x.ValueId == valueId)?
                                      .Id;
            }
        }
    }
}
