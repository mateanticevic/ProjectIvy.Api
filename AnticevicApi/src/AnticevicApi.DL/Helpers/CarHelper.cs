using AnticevicApi.DL.DbContexts;
using System.Linq;

namespace AnticevicApi.DL.Helpers
{
    public class CarHelper
    {
        public static int GetId(string valueId)
        {
            using (var db = new MainContext(""))
            {
                return db.Cars.SingleOrDefault(x => x.ValueId == valueId).Id;
            }
        }
    }
}
