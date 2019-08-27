using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Income;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.Income;

namespace ProjectIvy.Business.Handlers.Income
{
    public interface IIncomeHandler : IHandler
    {
        PagedView<View.Income> Get(IncomeGetBinding binding);

        int GetCount(FilteredBinding binding);

        Task<decimal> GetSum(IncomeGetSumBinding binding);

        IEnumerable<GroupedByMonth<decimal>> GetSumByMonth(IncomeGetSumBinding binding);

        IEnumerable<GroupedByYear<decimal>> GetSumByYear(IncomeGetSumBinding binding);
    }
}
