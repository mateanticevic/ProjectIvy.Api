﻿using Microsoft.AspNetCore.Http;
using ProjectIvy.Api.Extensions;
using ProjectIvy.Business.Cache;
using ProjectIvy.Business.Exceptions;
using ProjectIvy.Business.Handlers.Security;
using ProjectIvy.Model.Constants.Database;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProjectIvy.Api.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISecurityHandler _securityHandler;

        public AuthenticationMiddleware(RequestDelegate next, ISecurityHandler securityHandler)
        {
            _next = next;
            _securityHandler = securityHandler;
        }

        public Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path.Value.Contains("token"))
                return _next(httpContext);

            try
            {
                string authorizationToken = httpContext.GetAuthorizationToken();

                var user = TokenCache.GetUser(authorizationToken);

                if (user == null)
                {
                    user = _securityHandler.GetUser(authorizationToken);
                    TokenCache.SetUser(user, authorizationToken);
                }

                httpContext.Items.Add("User", user);
                httpContext.Items.Add("Token", authorizationToken);

                var claimIdentity = new ClaimsIdentity();

                claimIdentity.AddClaim(new Claim(ClaimTypes.Role, UserRole.Administrator));
                claimIdentity.AddClaim(new Claim(ClaimTypes.Role, UserRole.User));

                httpContext.User.AddIdentity(claimIdentity);

                return _next(httpContext);
            }
            catch(System.Exception e)
            {
                throw new UnauthorizedException();
            }
        }
    }
}
