using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ProjectIvy.Data.DbContexts;
using System.Linq;

namespace ProjectIvy.Business.Handlers
{
    public abstract class Handler<THandler> : IHandler
    {
        private static IDictionary<string, int> _identifierUserMapping;

        public Handler(IHandlerContext<THandler> context)
        {
            HttpContext = context.Context.HttpContext;
            Logger = context.Logger;

            string authIdentifier = HttpContext.User.Claims.Single(x => x.Type == "sub").Value;
            UserId = ResolveUserId(authIdentifier);
        }

        public Handler(IHandlerContext<THandler> context,
                       IMemoryCache memoryCache) : this(context)
        {
            MemoryCache = memoryCache;
        }

        public HttpContext HttpContext { get; set; }

        public ILogger Logger { get; set; }

        protected IMemoryCache MemoryCache { get; private set; }

        protected int UserId { get; private set; }

        protected MainContext GetMainContext() => new MainContext(Environment.GetEnvironmentVariable("CONNECTION_STRING_MAIN"));

        protected SqlConnection GetSqlConnection() => new SqlConnection(Environment.GetEnvironmentVariable("CONNECTION_STRING_MAIN"));

        protected string BuildUserCacheKey(string resourceKey) => $"{UserId}_{resourceKey}";

        private int ResolveUserId(string authIdentifier)
        {
            if (_identifierUserMapping is null || !_identifierUserMapping.ContainsKey(authIdentifier))
            {
                using (var db = GetMainContext())
                {
                    _identifierUserMapping = db.Users.Where(x => x.AuthIdentifier != null)
                                                     .ToDictionary(x => x.AuthIdentifier, x => x.Id);
                }
            }

            return _identifierUserMapping[authIdentifier];
        }
    }
}
