using AnticevicApi.Model.View.ExpenseType;
using System.Collections.Generic;
using System.Linq;

namespace AnticevicApi.BL.Handlers.Expense
{
    public class ExpenseTypeHandler : Handler<ExpenseTypeHandler>, IExpenseTypeHandler
    {
        public ExpenseTypeHandler(IHandlerContext<ExpenseTypeHandler> context) : base(context)
        {

        }

        public IEnumerable<ExpenseType> Get()
        {
            using (var db = GetMainContext())
            {
                return db.ExpenseTypes.OrderBy(x => x.TypeDescription)
                                      .ToList()
                                      .Select(x => new ExpenseType(x));
            }
        }
    }
}
