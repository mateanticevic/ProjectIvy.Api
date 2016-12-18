using AnticevicApi.BL.Handlers.Security;
using AnticevicApi.Cache;
using AnticevicApi.Model.Constants.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace AnticevicApi.Middleware
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
            try
            {
                string token = httpContext.Request.Headers.SingleOrDefault(x => x.Key == "Authorization").Value;

                var user = TokenCache.GetUser(token);

                if (user == null)
                {
                    user = _securityHandler.GetUser(token);
                    TokenCache.SetUser(user, token);
                }
                httpContext.Items.Add("User", user);

                var claimIdentity = new ClaimsIdentity();

                claimIdentity.AddClaim(new Claim(ClaimTypes.Role, UserRole.Administrator));

                httpContext.User.AddIdentity(claimIdentity);

                return _next(httpContext);
            }
            catch(Exception e)
            {
                return _next(httpContext);
            }
        }
    }

    public static class AuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}
