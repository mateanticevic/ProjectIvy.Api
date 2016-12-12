using AnticevicApi.BL.Handlers;
using AnticevicApi.Cache;
using AnticevicApi.DL.Helpers;
using AnticevicApi.Model.Constants.Database;
using AnticevicApi.Model.Database.Main.Security;
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

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            try
            {
                string token = httpContext.Request.Headers.SingleOrDefault(x => x.Key == "Authorization").Value;

                var accessToken = TokenCache.Get(token);

                if (accessToken == null)
                {
                    accessToken = AccessTokenHelper.Get(token);
                    TokenCache.Set(accessToken);
                }

                httpContext.Items.Add(nameof(AccessToken), accessToken);

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
