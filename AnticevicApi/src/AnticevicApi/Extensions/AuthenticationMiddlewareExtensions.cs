using AnticevicApi.Middleware;
using Microsoft.AspNetCore.Builder;

namespace AnticevicApi.Extensions
{
    public static class AuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}
