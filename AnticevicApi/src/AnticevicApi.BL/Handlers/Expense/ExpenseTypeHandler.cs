using AnticevicApi.DL.DbContexts;
using AnticevicApi.Model.View.ExpenseType;
using System.Collections.Generic;
using System.Linq;

namespace AnticevicApi.BL.Handlers.Expense
{
    public class ExpenseTypeHandler : Handler, IExpenseTypeHandler
    {
        public IEnumerable<ExpenseType> Get()
        {
            using (var db = new MainContext(ConnectionString))
            {
                return db.ExpenseTypes.OrderBy(x => x.TypeDescription)
                                      .ToList()
                                      .Select(x => new ExpenseType(x));
            }
        }
    }
}
