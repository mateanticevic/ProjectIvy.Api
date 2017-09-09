using ProjectIvy.Model.Binding.Common;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Income;

namespace ProjectIvy.BL.Handlers.Income
{
    public interface IIncomeHandler : IHandler
    {
        PagedView<View.Income> Get(FilteredPagedBinding binding);

        int GetCount(FilteredBinding binding);

        decimal GetSum(FilteredBinding binding, string currencyCode);
    }
}
