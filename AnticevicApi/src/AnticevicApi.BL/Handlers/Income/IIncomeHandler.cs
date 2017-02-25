using AnticevicApi.Model.Binding.Common;
using System.Collections.Generic;
using System;
using View = AnticevicApi.Model.View.Income;

namespace AnticevicApi.BL.Handlers.Income
{
    public interface IIncomeHandler : IHandler
    {
        IEnumerable<View.Income> Get(FilteredPagedBinding binding);

        int GetCount(DateTime from, DateTime to);

        decimal GetSum(FilteredBinding binding, string currencyCode);
    }
}
