using AnticevicApi.DL.DbContexts;
using AnticevicApi.Model.View.ExpenseType;
using System.Collections.Generic;
using System.Linq;

namespace AnticevicApi.BL.Handlers
{
    public class ExpenseTypeHandler : Handler
    {
        public ExpenseTypeHandler(string connectionString, int userId) : base(connectionString, userId)
        {
        }

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
