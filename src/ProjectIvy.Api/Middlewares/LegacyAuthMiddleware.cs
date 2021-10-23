using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Security;

namespace ProjectIvy.Api.Middlewares
{
    public class LegacyAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAccessTokenHandler _accessTokenHandler;
        private static IDictionary<string, string> _bearerMapping;

        public LegacyAuthMiddleware(RequestDelegate next, IAccessTokenHandler accessTokenHandler)
        {
            _next = next;
            _accessTokenHandler = accessTokenHandler;
        }

        public async Task Invoke(HttpContext httpContext, ILogger<LegacyAuthMiddleware> logger)
        {
            try
            {
                if (_bearerMapping is null)
                    _bearerMapping = await _accessTokenHandler.GetBearers();

                if (httpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    string authHeader = httpContext.Request.Headers["Authorization"].ToString();
                    bool hasScheme = authHeader.Contains(" ");
                    if (authHeader.Contains("Token") || !hasScheme)
                    {
                        string token = hasScheme ? authHeader.Substring(authHeader.IndexOf(' ') + 1) : authHeader;

                        if (_bearerMapping.ContainsKey(token))
                            httpContext.Request.Headers["Authorization"] = $"Bearer {_bearerMapping[token]}";
                    }
                }

                await _next(httpContext);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Legacy auth middleware failed");
            }
        }
    }
}
