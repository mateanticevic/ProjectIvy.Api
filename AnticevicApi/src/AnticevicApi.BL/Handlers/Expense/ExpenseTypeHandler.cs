using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.Binding.ExpenseType;
using AnticevicApi.Model.View.ExpenseType;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace AnticevicApi.BL.Handlers.Expense
{
    public class ExpenseTypeHandler : Handler<ExpenseTypeHandler>, IExpenseTypeHandler
    {
        public ExpenseTypeHandler(IHandlerContext<ExpenseTypeHandler> context) : base(context)
        {
        }

        public IEnumerable<ExpenseType> Get(ExpenseTypeGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                int? parentId = db.ExpenseTypes.GetId(binding.ParentId);

                return db.ExpenseTypes.Include(x => x.Children)
                                      .WhereIf(binding.HasChildren.HasValue, x => binding.HasChildren.Value ? x.Children.Any() : !x.Children.Any())
                                      .WhereIf(binding.HasParent.HasValue, x => binding.HasParent.Value ? x.ParentTypeId != null : x.ParentTypeId == null)
                                      .WhereIf(parentId.HasValue, x => x.ParentTypeId == parentId.Value)
                                      .OrderBy(x => x.TypeDescription)
                                      .ToList()
                                      .Select(x => new ExpenseType(x));
            }
        }
    }
}
