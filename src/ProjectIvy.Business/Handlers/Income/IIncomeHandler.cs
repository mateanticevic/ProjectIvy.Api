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

        Task<IEnumerable<View.IncomeSource>> GetSources();

        Task<decimal> GetSum(IncomeGetSumBinding binding);

        IEnumerable<GroupedByMonth<decimal>> GetSumByMonth(IncomeGetSumBinding binding);

        IEnumerable<KeyValuePair<int, decimal>> GetSumByYear(IncomeGetSumBinding binding);

        Task<IEnumerable<View.IncomeType>> GetTypes();
    }
}
