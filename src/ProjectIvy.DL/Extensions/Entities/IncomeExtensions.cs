using ProjectIvy.Model.Binding.Income;
using ProjectIvy.Model.Database.Main.Finance;
using System.Linq;

namespace ProjectIvy.DL.Extensions.Entities
{
    public static class IncomeExtensions
    {
        public static IOrderedQueryable<Income> OrderBy(this IQueryable<Income> query, IncomeGetBinding binding)
        {
            switch (binding.OrderBy)
            {
                case IncomeSort.Date:
                    return query.OrderBy(binding.OrderAscending, x => x.Timestamp);
                case IncomeSort.Amount:
                    return query.OrderBy(binding.OrderAscending, x => x.Ammount);
                default:
                    return query.OrderBy(binding.OrderAscending, x => x.Timestamp);
            }
        }
    }
}
