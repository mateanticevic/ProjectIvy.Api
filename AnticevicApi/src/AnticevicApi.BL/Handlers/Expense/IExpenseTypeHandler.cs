using AnticevicApi.Model.View.ExpenseType;
using System.Collections.Generic;

namespace AnticevicApi.BL.Handlers.Expense
{
    public interface IExpenseTypeHandler : IHandler
    {
        IEnumerable<ExpenseType> Get();
    }
}
