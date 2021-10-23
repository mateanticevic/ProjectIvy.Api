using Microsoft.AspNetCore.Builder;
using ProjectIvy.Api.Middlewares;

namespace ProjectIvy.Api.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }

        public static IApplicationBuilder UseLegacyAuth(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LegacyAuthMiddleware>();
        }
    }
}
