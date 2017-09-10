using ProjectIvy.Model.Binding.Common;
using ProjectIvy.Model.Database.Main;
using System.Linq.Expressions;
using System.Linq;
using System;

namespace ProjectIvy.DL.Extensions
{
    public static class IQueryableExtensions
    {
        public static int? GetId<T>(this IQueryable<T> query, string valueId) where T : class, IHasValueId
        {
            return query.SingleOrDefault(x => x.ValueId == valueId)?.Id;
        }

        public static IQueryable<T> InsideRectangle<T>(this IQueryable<T> query, LocationBinding a, LocationBinding b) where T : IHasLocation
        {
            if (a == null || b == null)
                return query;

            decimal minLat = a.Lat < b.Lat ? a.Lat : b.Lat;
            decimal maxLat = a.Lat > b.Lat ? a.Lat : b.Lat;

            decimal minLng = a.Lng < b.Lng ? a.Lng : b.Lng;
            decimal maxLng = a.Lng > b.Lng ? a.Lng : b.Lng;

            query = query.Where(x => x.Latitude > minLat)
                         .Where(x => x.Latitude < maxLat)
                         .Where(x => x.Longitude > minLng)
                         .Where(x => x.Longitude < maxLng);

            return query;
        }

        public static IOrderedQueryable<T> OrderBy<T,TKey>(this IQueryable<T> query, bool orderAscending, Expression<Func<T, TKey>> sortExpression)
        {
            return orderAscending ? query.OrderBy(sortExpression) : query.OrderByDescending(sortExpression);
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> query, FilteredPagedBinding binding)
        {
            return query.Page(binding.Page, binding.PageSize);
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> query, PagedBinding binding)
        {
            return query.Page(binding.Page, binding.PageSize);
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> query, int? page, int? pageSize = 10)
        {
            if (!pageSize.HasValue)
                return query;

            // Page number to page index (zero based)
            int pageIndex = page.HasValue ? page.Value - 1 : 0;
            pageIndex = pageIndex < 0 ? 0 : pageIndex;

            return query.Skip(pageIndex * pageSize.Value)
                        .Take(pageSize.Value);
        }

        public static IQueryable<T> WhereTimestampInclusive<T>(this IQueryable<T> query, FilteredBinding binding) where T : IHasTimestamp
        {
            return query.WhereTimestampInclusive(binding.From, binding.To);
        }

        public static IQueryable<T> WhereTimestampInclusive<T>(this IQueryable<T> query, DateTime? from, DateTime? to) where T : IHasTimestamp
        {
            query = from == null ? query : query.Where(x => x.Timestamp >= from);
            query = to == null ? query : query.Where(x => x.Timestamp <= to);

            return query;
        }

        public static IQueryable<T> WhereTimestampFromInclusive<T>(this IQueryable<T> query, DateTime? from, DateTime? to) where T : IHasTimestamp
        {
            query = from == null ? query : query.Where(x => x.Timestamp >= from);
            query = to == null ? query : query.Where(x => x.Timestamp < to);

            return query;
        }

        public static T SingleOrDefault<T>(this IQueryable<T> query, string valueId) where T : IHasValueId
        {
            return query.SingleOrDefault(x => x.ValueId == valueId);
        }

        public static IQueryable<T> WhereIf<T>(this IQueryable<T> queryable, bool ifTrue, Expression<Func<T, bool>> condition)
        {
            return ifTrue ? queryable.Where(condition) : queryable;
        }
    }
}
