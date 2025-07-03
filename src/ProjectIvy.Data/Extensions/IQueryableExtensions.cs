using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Common;
using ProjectIvy.Model.Binding.Route;
using ProjectIvy.Model.Database.Main;
using ProjectIvy.Model.View;

namespace ProjectIvy.Data.Extensions;

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

        decimal minLat = a.Latitude < b.Latitude ? a.Latitude : b.Latitude;
        decimal maxLat = a.Latitude > b.Latitude ? a.Latitude : b.Latitude;

        decimal minLng = a.Longitude < b.Longitude ? a.Longitude : b.Longitude;
        decimal maxLng = a.Longitude > b.Longitude ? a.Longitude : b.Longitude;

        query = query.Where(x => x.Latitude > minLat)
                     .Where(x => x.Latitude < maxLat)
                     .Where(x => x.Longitude > minLng)
                     .Where(x => x.Longitude < maxLng);

        return query;
    }

    public static IOrderedQueryable<T> OrderBy<T, TKey>(this IQueryable<T> query, bool orderAscending, Expression<Func<T, TKey>> sortExpression)
    {
        return orderAscending ? query.OrderBy(sortExpression) : query.OrderByDescending(sortExpression);
    }

    public static IQueryable<T> Page<T>(this IQueryable<T> query, IPagedBinding binding)
    {
        if (binding.PageAll)
            return query;

        // Page number to page index (zero based)
        int pageIndex = binding.Page - 1;
        pageIndex = pageIndex < 0 ? 0 : pageIndex;

        return query.Skip(pageIndex * binding.PageSize)
                    .Take(binding.PageSize);
    }

    public static PagedView<T> ToPagedView<T>(this IQueryable<T> query, IPagedBinding binding, long? count = null)
    {
        return new PagedView<T>()
        {
            Count = count ?? query.Count(),
            Items = query.Page(binding).ToList()
        };
    }

    public static async Task<PagedView<T>> ToPagedViewAsync<T>(this IQueryable<T> query, IPagedBinding binding, long? count = null)
    {
        return new PagedView<T>()
        {
            Count = count ?? query.Count(),
            Items = await query.Page(binding).ToListAsync()
        };
    }

    public static IQueryable<T> WhereCreatedInclusive<T>(this IQueryable<T> query, IFilteredBinding binding) where T : IHasCreated
    {
        return query.WhereCreatedInclusive(binding.From, binding.To);
    }

    public static IQueryable<T> WhereCreatedInclusive<T>(this IQueryable<T> query, DateTime? from, DateTime? to) where T : IHasCreated
    {
        query = from == null ? query : query.Where(x => x.Created >= from);
        query = to == null ? query : query.Where(x => x.Created <= to);

        return query;
    }

    public static IQueryable<T> WhereTimestampInclusive<T>(this IQueryable<T> query, IFilteredBinding binding) where T : IHasTimestamp
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
        => query.SingleOrDefault(x => x.ValueId == valueId);

    public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> queryable, bool ifTrue, Func<T, bool> condition)
        => ifTrue ? queryable.Where(condition) : queryable;

    public static IQueryable<T> WhereIf<T>(this IQueryable<T> queryable, bool ifTrue, Expression<Func<T, bool>> condition)
        => ifTrue ? queryable.Where(condition) : queryable;

    public static IQueryable<T> WhereIf<T>(this IQueryable<T> queryable, object ifNotNull, Expression<Func<T, bool>> condition)
        => ifNotNull != null ? queryable.Where(condition) : queryable;

    public static IQueryable<T> WhereIf<T, TItem>(this IQueryable<T> queryable, IEnumerable<TItem> ifHasItems, Expression<Func<T, bool>> condition)
        => ifHasItems != null && ifHasItems.Any() ? queryable.Where(condition) : queryable;

    public static IQueryable<TItem> WhereSearch<TBinding, TItem>(this IQueryable<TItem> query, TBinding binding) where TBinding : ISearchable where TItem : IHasName, IHasValueId
        => query.WhereIf(!string.IsNullOrEmpty(binding.Search), x => x.Name.ToLower().Contains(binding.Search.ToLower()) || x.ValueId.ToLower().Contains(binding.Search.ToLower()));
}
