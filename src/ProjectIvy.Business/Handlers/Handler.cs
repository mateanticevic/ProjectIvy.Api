using System.Linq;
using System.Security.Claims;
using ProjectIvy.Business.Exceptions;
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

        var emails = HttpContext?.User.FindAll(ClaimTypes.Email).Concat(HttpContext.User.FindAll("email")).Select(c => c.Value).Distinct().ToArray();
        if (HttpContext?.User.Identity?.IsAuthenticated != true || emails?.Length != 1 || string.IsNullOrWhiteSpace(emails[0]))
            throw new UnauthorizedException();
        string authIdentifier = emails[0];
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

    protected void AddCacheKey(string newCacheKey)
    {
        string cacheKey = BuildUserCacheKey(_resourceCacheKey);
        var cacheKeys = MemoryCache.Get<IEnumerable<string>>(cacheKey);
        var updatedCacheKeys = cacheKeys?.ToList() ?? new List<string>();
        updatedCacheKeys.Add(newCacheKey);

        MemoryCache.Set(cacheKey, updatedCacheKeys.Distinct().AsEnumerable());
    }

    protected string BuildUserCacheKey(string resourceKey) => $"{UserId}_{resourceKey}";

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

    protected MainContext GetMainContext() => new MainContext(Environment.GetEnvironmentVariable("CONNECTION_STRING_MAIN"));

    protected SqlConnection GetSqlConnection() => new SqlConnection(Environment.GetEnvironmentVariable("CONNECTION_STRING_MAIN"));

    private int ResolveUserId(string email)
    {
        if (_identifierUserMapping is null || !_identifierUserMapping.ContainsKey(email))
        {
            using var db = GetMainContext();
            _identifierUserMapping = db.Users.Where(x => x.Email != null)
                                             .ToDictionary(x => x.Email, x => x.Id);
        }

        if (!_identifierUserMapping.TryGetValue(email, out var userId))
            throw new ResourceForbiddenException();
        return userId;
    }
}
