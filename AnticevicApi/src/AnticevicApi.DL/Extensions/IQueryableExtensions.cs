using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.Database.Main;
using System.Linq;

namespace AnticevicApi.DL.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Page<T>(this IQueryable<T> query, FilteredPagedBinding binding)
        {
            return query.Page(binding.Page, binding.PageSize);
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> query, PagedBinding binding)
        {
            return query.Page(binding.Page, binding.PageSize);
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> query, int? page, int? pageSize)
        {
            return query.Skip(page.Value * pageSize.Value)
                        .Take(pageSize.Value);
        }

        public static IQueryable<T> WhereTimestampInclusive<T>(this IQueryable<T> query, FilteredBinding binding) where T : IHasTimestamp
        {
            query = binding?.From == null ? query : query.Where(x => x.Timestamp >= binding.From);
            query = binding?.To == null ? query : query.Where(x => x.Timestamp <= binding.To);

            return query;
        }

        public static T SingleOrDefault<T>(this IQueryable<T> query, string valueId) where T : IHasValueId
        {
            return query.SingleOrDefault(x => x.ValueId == valueId);
        }
    }
}
