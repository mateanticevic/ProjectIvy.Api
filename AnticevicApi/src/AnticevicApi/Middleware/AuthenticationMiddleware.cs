using AnticevicApi.BL.Handlers.Security;
using AnticevicApi.Cache;
using AnticevicApi.Extensions;
using AnticevicApi.Model.Constants.Database;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
                string authorizationToken = httpContext.GetAuthorizationToken();

                var user = TokenCache.GetUser(authorizationToken);

                if (user == null)
                {
                    user = _securityHandler.GetUser(authorizationToken);
                    TokenCache.SetUser(user, authorizationToken);
                }

                httpContext.Items.Add("User", user);

                var claimIdentity = new ClaimsIdentity();

                claimIdentity.AddClaim(new Claim(ClaimTypes.Role, UserRole.Administrator));

                httpContext.User.AddIdentity(claimIdentity);

                return _next(httpContext);
            }
            catch
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
