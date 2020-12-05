using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using ProjectIvy.Data.DbContexts;
using System;

namespace ProjectIvy.Business.Handlers
{
    public abstract class Handler<THandler> : IHandler
    {
        public Handler(IHandlerContext<THandler> context)
        {
            HttpContext = context.Context.HttpContext;
            User = HttpContext != null ? (Model.Database.Main.User.User)HttpContext.Items["User"] : null;
            Logger = context.Logger;
            AccessToken = HttpContext != null ? (string)HttpContext.Items["Token"] : null;
        }

        public HttpContext HttpContext { get; set; }

        public ILogger Logger { get; set; }

        protected Model.Database.Main.User.User User { get; private set; }

        protected string AccessToken { get; private set; }

        protected MainContext GetMainContext() => new MainContext(Environment.GetEnvironmentVariable("CONNECTION_STRING_MAIN"));

        protected SqlConnection GetSqlConnection() => new SqlConnection(Environment.GetEnvironmentVariable("CONNECTION_STRING_MAIN"));
    }
}
