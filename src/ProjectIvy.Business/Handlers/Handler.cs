using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using ProjectIvy.Data.DbContexts;
using System;
using System.Linq;

namespace ProjectIvy.Business.Handlers
{
    public abstract class Handler<THandler> : IHandler
    {
        public Handler(IHandlerContext<THandler> context)
        {
            HttpContext = context.Context.HttpContext;
            if (HttpContext is not null)
            {
                string userId = HttpContext.User.Claims.Single(x => x.Type == "userid").Value;
                UserId = Convert.ToInt32(userId);
            }

            Logger = context.Logger;
            AccessToken = HttpContext != null ? (string)HttpContext.Items["Token"] : null;
        }

        public HttpContext HttpContext { get; set; }

        public ILogger Logger { get; set; }

        protected int? UserId { get; private set; }

        protected string AccessToken { get; private set; }

        protected MainContext GetMainContext() => new MainContext(Environment.GetEnvironmentVariable("CONNECTION_STRING_MAIN"));

        protected SqlConnection GetSqlConnection() => new SqlConnection(Environment.GetEnvironmentVariable("CONNECTION_STRING_MAIN"));
    }
}
