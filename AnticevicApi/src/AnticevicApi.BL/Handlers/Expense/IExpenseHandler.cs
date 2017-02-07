using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.Binding.Expense;
using AnticevicApi.Model.Constants;
using AnticevicApi.Model.View;
using System.Collections.Generic;
using System;
using View = AnticevicApi.Model.View.Expense;

namespace AnticevicApi.BL.Handlers.Expense
{
    public interface IExpenseHandler : IHandler
    {
        string Create(ExpenseBinding binding);

        bool Delete(string valueId);

        PaginatedView<View.Expense> Get(DateTime? from, DateTime? to, string expenseTypeValueId, string vendorValueId, int? page = 0, int? pageSize = 10);

        IEnumerable<View.Expense> GetByDate(DateTime date);

        IEnumerable<View.Expense> GetByType(string valueId, DateTime? from = default(DateTime?), DateTime? to = default(DateTime?));

        IEnumerable<View.Expense> GetByVendor(string valueId, DateTime? from = default(DateTime?), DateTime? to = default(DateTime?));

        int GetCount(FilteredBinding binding);

        IEnumerable<KeyValuePair<DateTime, decimal>> GetGroupedSum(string typeValueId, TimeGroupingTypes timeGroupingTypes);

        decimal GetSum(FilteredBinding binding, string currencyCode);

        bool Update(ExpenseBinding binding);
    }
}
