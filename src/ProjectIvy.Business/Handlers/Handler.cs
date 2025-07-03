using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ProjectIvy.Data.DbContexts;

namespace ProjectIvy.Business.Handlers;

public abstract class Handler<THandler> : IHandler
{
    private static IDictionary<string, int> _identifierUserMapping;

    private readonly string _resourceCacheKey;

    public Handler(IHandlerContext<THandler> context)
    {
        HttpContext = context.Context.HttpContext;
        Logger = context.Logger;

        string authIdentifier = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
        UserId = ResolveUserId(authIdentifier);
    }

    public Handler(IHandlerContext<THandler> context,
                   IMemoryCache memoryCache,
                   string resourceCacheKey) : this(context)
    {
        MemoryCache = memoryCache;
        _resourceCacheKey = resourceCacheKey;
    }

    public HttpContext HttpContext { get; set; }

    public ILogger Logger { get; set; }

    protected IMemoryCache MemoryCache { get; private set; }

    protected int UserId { get; private set; }

    protected MainContext GetMainContext() => new MainContext(Environment.GetEnvironmentVariable("CONNECTION_STRING_MAIN"));

    protected SqlConnection GetSqlConnection() => new SqlConnection(Environment.GetEnvironmentVariable("CONNECTION_STRING_MAIN"));

    protected string BuildUserCacheKey(string resourceKey) => $"{UserId}_{resourceKey}";

    protected void AddCacheKey(string newCacheKey)
    {
        string cacheKey = BuildUserCacheKey(_resourceCacheKey);
        var cacheKeys = MemoryCache.Get<IEnumerable<string>>(cacheKey);
        var updatedCacheKeys = cacheKeys?.ToList() ?? new List<string>();
        updatedCacheKeys.Add(newCacheKey);

        MemoryCache.Set(cacheKey, updatedCacheKeys.Distinct().AsEnumerable());
    }

    protected void ClearCache()
    {
        string cacheKey = BuildUserCacheKey(_resourceCacheKey);
        var keys = MemoryCache.Get<IEnumerable<string>>(cacheKey);

        if (keys is not null)
        {
            foreach (var key in keys)
            {
                MemoryCache.Remove(key);
            }
            MemoryCache.Remove(cacheKey);
        }
    }

    private int ResolveUserId(string email)
    {
        if (_identifierUserMapping is null || !_identifierUserMapping.ContainsKey(email))
        {
            using (var db = GetMainContext())
            {
                _identifierUserMapping = db.Users.Where(x => x.Email != null)
                                                 .ToDictionary(x => x.Email, x => x.Id);
            }
        }

        return _identifierUserMapping[email];
    }
}
