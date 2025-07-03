using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProjectIvy.Business.Caching;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Model.Binding.Beer;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Beer;

namespace ProjectIvy.Business.Handlers.Beer;

public class BeerHandler : Handler<BeerHandler>, IBeerHandler
{
    public BeerHandler(IHandlerContext<BeerHandler> context, IMemoryCache memoryCache)
        : base(context, memoryCache, nameof(BeerHandler)) { }

    public async Task<string> CreateBeer(string brandValueId, BeerBinding binding)
    {
        using (var context = GetMainContext())
        {
            binding.BrandId = brandValueId;
            var entity = binding.ToEntity(context);
            await context.Beers.AddAsync(entity);
            await context.SaveChangesAsync();

            return entity.ValueId;
        }
    }

    public async Task<string> CreateBrand(BrandBinding binding)
    {
        using (var context = GetMainContext())
        {
            var entity = binding.ToEntity(context);

            await context.BeerBrands.AddAsync(entity);
            await context.SaveChangesAsync();
            ClearCache();

            return entity.ValueId;
        }
    }

    public async Task<View.Beer> GetBeer(string id)
    {
        using (var context = GetMainContext())
        {
            var beers = await context
                .Beers.Include(x => x.BeerStyle)
                .SingleOrDefaultAsync(x => x.ValueId == id);

            return beers.ConvertTo(x => new View.Beer(x));
        }
    }

    public async Task<PagedView<View.Beer>> GetBeers(BeerGetBinding binding)
    {
        using (var context = GetMainContext())
        {
            int? brandId = context.BeerBrands.GetId(binding.BrandId);

            return await context
                .Beers.Include(x => x.BeerStyle)
                .Include(x => x.BeerBrand)
                .WhereIf(brandId.HasValue, x => x.BeerBrandId == brandId)
                .OrderBy(binding)
                .WhereIf(
                    !string.IsNullOrWhiteSpace(binding.Search),
                    x =>
                        x.Name.ToLower().Contains(binding.Search.ToLower())
                        || x.ValueId.ToLower().Contains(binding.Search.ToLower())
                )
                .Select(x => new View.Beer(x))
                .ToPagedViewAsync(binding);
        }
    }

    public async Task<IEnumerable<View.BeerBrand>> GetBrands(BrandGetBinding binding)
    {
        string cacheKey = BuildUserCacheKey(CacheKeyGenerator.BeerBrandsGet(binding));
        return await MemoryCache.GetOrCreateAsync(
            cacheKey,
            async cacheEntry =>
            {
                AddCacheKey(cacheKey);
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

                return await GetBrandsNonCached(binding);
            }
        );
    }

    public async Task<IEnumerable<View.BeerServing>> GetServings()
    {
        using (var context = GetMainContext())
        {
            return await context.BeerServings.Select(x => new View.BeerServing(x)).ToListAsync();
        }
    }

    public async Task<IEnumerable<View.BeerStyle>> GetStyles()
    {
        using (var context = GetMainContext())
        {
            return await context
                .BeerStyles.OrderBy(x => x.Name)
                .Select(x => new View.BeerStyle(x))
                .ToListAsync();
        }
    }

    public async Task UpdateBeer(string id, BeerBinding binding)
    {
        using (var context = GetMainContext())
        {
            var beer = await context.Beers.SingleOrDefaultAsync(x => x.ValueId == id);
            var entity = binding.ToEntity(context, beer);

            await context.SaveChangesAsync();
        }
    }

    public async Task UpdateBrand(string id, BrandBinding binding)
    {
        using (var context = GetMainContext())
        {
            var brand = await context.BeerBrands.SingleOrDefaultAsync(x => x.ValueId == id);
            var entity = binding.ToEntity(context, brand);

            await context.SaveChangesAsync();
            ClearCache();
        }
    }

    public async Task<IEnumerable<View.BeerBrand>> GetBrandsNonCached(BrandGetBinding binding)
    {
        using (var context = GetMainContext())
        {
            return await context
                .BeerBrands.Include(x => x.Country)
                .WhereIf(
                    binding.HasCountry.HasValue,
                    x => !(binding.HasCountry.Value ^ x.CountryId.HasValue)
                )
                .WhereIf(
                    !string.IsNullOrWhiteSpace(binding.Search),
                    x =>
                        x.Name.ToLower().Contains(binding.Search.ToLower())
                        || x.ValueId.ToLower().Contains(binding.Search.ToLower())
                )
                .OrderBy(x => x.Name)
                .Select(x => new View.BeerBrand(x))
                .ToListAsync();
        }
    }
}
