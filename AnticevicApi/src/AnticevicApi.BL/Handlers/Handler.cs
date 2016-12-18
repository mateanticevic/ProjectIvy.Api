using AnticevicApi.Common.Configuration;
using AnticevicApi.DL.DbContexts;
using AnticevicApi.Model.Database.Main.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AnticevicApi.BL.Handlers
{
    public abstract class Handler<THandler> : IHandler
    {
        public Handler()
        {

        }

        public Handler(IHandlerContext<THandler> context)
        {
            HttpContext = context.Context.HttpContext;
            UserId = ((AccessToken)HttpContext.Items["AccessToken"]).UserId;
            Logger = context.Logger;
            Settings = context.Settings;
        }

        public Handler(string connectionString, int userId)
        {
            ConnectionString = connectionString;
            UserId = userId;
        }

        public void Initialize(string connectionString, int userId, ILogger logger)
        {
            if(!IsInitialized)
            {
                ConnectionString = connectionString;
                Logger = logger;
                UserId = userId;

                IsInitialized = true;
            }
        }

        public string ConnectionString { get; set; }
        public int UserId { get; set; }
        public bool IsInitialized { get; private set; }
        public ILogger Logger { get; set; }
        public IOptions<AppSettings> Settings { get; set; }
        public HttpContext HttpContext { get; set; }

        protected MainContext GetMainContext()
        {
            return new MainContext(Settings.Value.ConnectionStrings.Main);
        }
    }
}
