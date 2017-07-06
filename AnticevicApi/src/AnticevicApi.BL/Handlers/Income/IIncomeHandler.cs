using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.View;
using View = AnticevicApi.Model.View.Income;

namespace AnticevicApi.BL.Handlers.Income
{
    public interface IIncomeHandler : IHandler
    {
        PagedView<View.Income> Get(FilteredPagedBinding binding);

        int GetCount(FilteredBinding binding);

        decimal GetSum(FilteredBinding binding, string currencyCode);
    }
}
