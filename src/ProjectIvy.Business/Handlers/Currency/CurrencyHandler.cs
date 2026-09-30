using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

    public async Task<IEnumerable<View.Currency>> Get()
        => await _memoryCache.GetOrCreateAsync(CacheKeyGenerator.CurrenciesGet(),
            async cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return await GetNonCached();
            });

    public async Task<View.Currency> Get(string code)
    {
        using var context = GetMainContext();
        var entity = await context.Currencies.SingleOrDefaultAsync(x => x.Code == code);

        return new View.Currency(entity);
    }

    private async Task<IEnumerable<View.Currency>> GetNonCached()
    {
        using var context = GetMainContext();
        return (await context.Currencies.OrderBy(x => x.Name)
                                 .ToListAsync())
                                 .Select(x => new View.Currency(x));
    }
}
