using Microsoft.AspNetCore.Builder;
using ProjectIvy.Api.Middlewares;
using Serilog;
using System.Reflection;

namespace ProjectIvy.Api.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }

    public static IApplicationBuilder UseSerilogRequestLoggingWithEnrichment(this IApplicationBuilder app, Assembly assembly)
    {
        app.UseSerilogRequestLogging(configure =>
        {
            configure.EnrichDiagnosticContext = (context, httpContext) =>
            {
                string authorizationValue = httpContext.Request.Headers["Authorization"];
                string cookieTokenValue = httpContext.Request.Cookies["Token"];
                string token = authorizationValue ?? cookieTokenValue;

                if (token is not null)
                {
                    string maskedToken = $"*****{token[^6..]}";
                    context.Set("Token", maskedToken);
                }
                context.Set("Version", assembly.GetName().Version.ToString());

                // Add all HTTP request headers as header_{headerName}
                foreach (var header in httpContext.Request.Headers)
                {
                    string headerName = header.Key.Replace("-", "");
                    string headerValue = header.Value.ToString();

                    // Mask sensitive headers
                    if (header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(headerValue))
                        headerValue = headerValue.Length > 6 ? $"*****{headerValue[^6..]}" : "*****";
                    else if (header.Key.Equals("Cookie", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(headerValue))
                        headerValue = "*****";

                    context.Set($"header_{headerName}", headerValue);
                }
            };
        });

        return app;
    }
}
