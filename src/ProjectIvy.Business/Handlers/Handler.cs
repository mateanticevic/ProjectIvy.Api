using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using ProjectIvy.Data.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectIvy.Business.Handlers
{
    public abstract class Handler<THandler> : IHandler
    {
        private static IDictionary<string, int> _identifierUserMapping;

        public Handler(IHandlerContext<THandler> context)
        {
            HttpContext = context.Context.HttpContext;
            if (HttpContext is not null)
            {
                string authIdentifier = HttpContext.User.Claims.Single(x => x.Type == "sub").Value;
                UserId = ResolveUserId(authIdentifier);
            }

            Logger = context.Logger;
            AccessToken = HttpContext != null ? (string)HttpContext.Items["Token"] : null;
        }

        public HttpContext HttpContext { get; set; }

        public ILogger Logger { get; set; }

        protected int UserId { get; private set; }

        protected string AccessToken { get; private set; }

        protected MainContext GetMainContext() => new MainContext(Environment.GetEnvironmentVariable("CONNECTION_STRING_MAIN"));

        protected SqlConnection GetSqlConnection() => new SqlConnection(Environment.GetEnvironmentVariable("CONNECTION_STRING_MAIN"));

        private int ResolveUserId(string authIdentifier)
        {
            if (_identifierUserMapping is null || !_identifierUserMapping.ContainsKey(authIdentifier))
            {
                using (var db = GetMainContext())
                {
                    _identifierUserMapping = db.Users.Where(x => x.AuthIdentifier != null).ToDictionary(x => x.AuthIdentifier, x => x.Id);
                }
            }

            return _identifierUserMapping[authIdentifier];
        }
    }
}
