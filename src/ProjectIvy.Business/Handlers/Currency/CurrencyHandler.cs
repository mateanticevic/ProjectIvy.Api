using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using ProjectIvy.Business.Caching;
using View = ProjectIvy.Model.View.Currency;

namespace ProjectIvy.Business.Handlers.Currency;

public class CurrencyHandler : Handler<CurrencyHandler>, ICurrencyHandler
{
    private readonly IMemoryCache _memoryCache;

    public CurrencyHandler(IHandlerContext<CurrencyHandler> context,
                           IMemoryCache memoryCache) : base(context)
    {
        _memoryCache = memoryCache;
    }

    public IEnumerable<View.Currency> Get()
        => _memoryCache.GetOrCreate(CacheKeyGenerator.CurrenciesGet(),
            cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return GetNonCached();
            });

    public View.Currency Get(string code)
    {
        using (var context = GetMainContext())
        {
            var entity = context.Currencies.SingleOrDefault(x => x.Code == code);

            return new View.Currency(entity);
        }
    }

    private IEnumerable<View.Currency> GetNonCached()
    {
        using (var context = GetMainContext())
        {
            return context.Currencies.OrderBy(x => x.Name)
                                     .ToList()
                                     .Select(x => new View.Currency(x));
        }
    }
}
