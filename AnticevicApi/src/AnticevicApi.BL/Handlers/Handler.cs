using AnticevicApi.Common.Configuration;
using AnticevicApi.DL.DbContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AnticevicApi.BL.Handlers
{
    public abstract class Handler<THandler> : IHandler
    {
        public Handler(IHandlerContext<THandler> context)
        {
            HttpContext = context.Context.HttpContext;
            User = HttpContext != null ? (Model.Database.Main.User.User)HttpContext.Items["User"] : null;
            Logger = context.Logger;
            Settings = context.Settings;
        }

        public HttpContext HttpContext { get; set; }
        public ILogger Logger { get; set; }
        public IOptions<AppSettings> Settings { get; set; }
        protected Model.Database.Main.User.User User { get; private set; }

        protected MainContext GetMainContext()
        {
            return new MainContext(Settings.Value.ConnectionStrings.Main);
        }
    }
}
