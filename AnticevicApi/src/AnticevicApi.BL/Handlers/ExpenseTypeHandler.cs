using AnticevicApi.DL.DbContexts;
using AnticevicApi.Model.View.ExpenseType;
using System.Collections.Generic;
using System.Linq;

namespace AnticevicApi.BL.Handlers
{
    public class ExpenseTypeHandler : Handler
    {
        public ExpenseTypeHandler(int userId)
        {
            UserId = userId;
        }

        public IEnumerable<ExpenseType> Get()
        {
            using (var db = new MainContext())
            {
                return db.ExpenseTypes.OrderBy(x => x.TypeDescription)
                                      .ToList()
                                      .Select(x => new ExpenseType(x));
            }
        }
    }
}
